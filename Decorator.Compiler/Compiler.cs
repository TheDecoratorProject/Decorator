using Decorator.Attributes;
using Decorator.Exceptions;
using Decorator.ModuleAPI;
using StrictEmit;
using SwissILKnife;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace Decorator.Compiler
{
	public delegate bool ILDeserialize<T>(object[] array, ref int i, out T result);
	public delegate object[] ILSerialize<T>(T item);

	public class Compiler<T> : ICompiler<T>
		where T : new()
	{
		public BaseModule[] Compile(Func<MemberInfo, BaseContainer> getContainer)
		{
			// cache constructor
			InstanceOf<T>.Create();

			// get all props/fields with pos attrib

			var members = DiscoverMembers();

			var dict = new SortedDictionary<int, BaseModule>();

			SetDecoratorModules(dict, members, getContainer);

			// fill up empty spaces with Ignored
			var last = dict.Keys.LastOrDefault();

			for (var i = 0; i < last; i++)
			{
				if (!dict.ContainsKey(i))
				{
					dict[i] = new IgnoredLogic();
				}
			}

			// save it as an array
			return dict.Values.ToArray();
		}

		public bool SupportsIL(BaseModule[] modules)
		{
			foreach(var i in modules)
			{
				if (!i.GetType()
					.GetInterfaces()
					.Contains(typeof(ILSupport)))
				{
					return false;
				}
			}

			return true;
		}

		public ILDeserialize<T> CompileILDeserialize(BaseModule[] modules)
		{
			var dm = new DynamicMethod<ILDeserialize<T>>(string.Empty,
				typeof(bool),
				new[]
				{
					typeof(object[]),
					typeof(int).MakeByRefType(),
					typeof(T).MakeByRefType()
				},
				true);

			var il = dm.ILGenerator;

			GenDesIL(il, modules);

			return dm.CreateDelegate();
		}

		public ILSerialize<T> CompileILSerialize(BaseModule[] modules)
		{
			var dm = new DynamicMethod<ILSerialize<T>>(string.Empty,
				typeof(object[]),
				new[]
				{
					typeof(T)
				},
				true);

			var il = dm.ILGenerator;

			GenSerIL(il, modules);

			return dm.CreateDelegate();
		}

#if NET45
		public static void SaveWrap(BaseModule[] modules)
		{
			var asm = System.Reflection.Emit.AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("TestAssembly"), System.Reflection.Emit.AssemblyBuilderAccess.RunAndSave);
			var mod = asm.DefineDynamicModule("TestModule", "asm.dll", true);
			var cls = mod.DefineType("SomeClass", TypeAttributes.Public | TypeAttributes.Class);
			var dm = cls.DefineMethod("Test", MethodAttributes.Public, typeof(bool), new[] { typeof(object[]), typeof(int).MakeByRefType(), typeof(T).MakeByRefType() });
			var il = dm.GetILGenerator();

			GenDesIL(il, modules);

			cls.CreateType();
			asm.Save("asm.dll", PortableExecutableKinds.ILOnly, ImageFileMachine.AMD64);
		}
		public static void SaveWrap2(BaseModule[] modules)
		{
			var asm = System.Reflection.Emit.AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("TestAssembly"), System.Reflection.Emit.AssemblyBuilderAccess.RunAndSave);
			var mod = asm.DefineDynamicModule("TestModule", "asm.dll", true);
			var cls = mod.DefineType("SomeClass", TypeAttributes.Public | TypeAttributes.Class);
			var dm = cls.DefineMethod("Test", MethodAttributes.Public, typeof(T), new[] { typeof(object[]) });
			var il = dm.GetILGenerator();

			GenSerIL(il, modules);

			cls.CreateType();
			asm.Save("asm.dll", PortableExecutableKinds.ILOnly, ImageFileMachine.AMD64);
		}
#endif

		private static void GenDesIL(System.Reflection.Emit.ILGenerator il, BaseModule[] modules)
		{
			// result = new T();
			il.EmitLoadArgument(2);
			il.EmitNewObject<T>();
			il.EmitSet(typeof(T));

			int c = 0;
			foreach (var member in GetILModules(modules))
			{
				var module = modules[c];

				var returnIfLessLabel = il.DefineLabel();

				il.EmitLoadArgument(0);
				il.EmitLoadArrayLength();
				il.EmitConvertToInt();
				il.EmitLoadArgument(1);
				il.EmitLoad<int>();
				il.EmitShortBranchGreaterThan(returnIfLessLabel);

				il.EmitConstantInt(0);
				il.EmitReturn();

				il.MarkLabel(returnIfLessLabel);

				member.GenerateDeserialize(il,
					() =>
					{
						il.EmitILForGetMethod(module.ModuleContainer.Member.GetMember, () =>
						{
							il.EmitLoadArgument(2);
						});
					},
					(v) =>
					{
						il.EmitILForSetMethod(module.ModuleContainer.Member.GetMember,
						() =>
						{
							il.EmitLoadArgument(2);
						},
						() =>
						{
							v();
						});
					},
					() =>
					{
						il.EmitLoadArgument(0);
						il.EmitLoadArgument(1);
						il.EmitLoad<int>();
						il.EmitLoadArrayElement<object>();
					},
					() =>
					{
						il.EmitLoadArgument(1);
					},
					(a) =>
					{
						il.EmitLoadArgument(1);
						il.EmitLoadArgument(1);
						il.EmitLoad<int>();
						il.EmitConstantInt(a);
						il.EmitAdd();
						il.EmitSet(typeof(int));
					});

				c++; //c#; >:(
			}

			il.EmitConstantInt(1);
			il.EmitReturn();
		}

		public static void GenSerIL(System.Reflection.Emit.ILGenerator il, BaseModule[] modules)
		{
			// get size
			var objSize = il.DeclareLocal(typeof(int));
			il.EmitConstantInt(0);
			il.EmitSetLocalVariable(objSize);

			foreach(var i in GetILModules(modules))
			{
				i.GenerateSerializeSize(il,
					() =>
					{
						il.EmitLoadArgument(0);
					},
					(load) =>
					{
						load();
						il.EmitLoadLocalVariable(objSize);
						il.EmitAdd();
						il.EmitSetLocalVariable(objSize);
					});
			}

			var objArray = il.DeclareLocal(typeof(object[]));
			var index = il.DeclareLocal(typeof(int));

			il.EmitConstantInt(0);
			il.EmitSetLocalVariable(index);

			il.EmitLoadLocalVariable(objSize);
			il.EmitNewArray(typeof(object));
			il.EmitSetLocalVariable(objArray);

			int c = 0;
			foreach(var i in GetILModules(modules))
			{
				i.GenerateSerialize(il, index,
					() =>
					{
						MemberUtils.EmitILForGetMethod(il, modules[c].ModuleContainer.Member.GetMember, () =>
						{
							il.EmitLoadArgument(0);
						});
					},
					(pushVal) =>
					{
						il.EmitLoadLocalVariable(objArray);
						il.EmitLoadLocalVariable(index);
						pushVal();
						il.EmitSetArrayElement<object>();
					});

				c++;
			}

			il.EmitLoadLocalVariable(objArray);
			il.EmitReturn();
		}

		private static IEnumerable<ILSupport> GetILModules(BaseModule[] modules)
		{
			foreach(var i in modules)
			{
				if (i is ILSupport ilSupport)
				{
					yield return ilSupport;
					continue;
				}

				throw new DecoratorCompilerException($"IL generation is not supported on this type.", null);
			};
		}

		private static void SetDecoratorModules(SortedDictionary<int, BaseModule> dictionary, IEnumerable<MemberInfo> members, Func<MemberInfo, BaseContainer> getContainer)
		{
			// for every member, get the DecoratorModule and store it in dict
			foreach (var i in members)
			{
				var builder = GetPairingOf(i);

				var decoratorModule =
					ModuleBuilder.Build(getContainer(i), builder);

				var positionAttribute = i.GetCustomAttributes()
											.OfType<PositionAttribute>()
											.First();

				if (positionAttribute.Position < 0)
				{
					throw ExceptionManager.GetIrrationalAttributeValue<PositionAttribute>
						(typeof(T), positionAttribute.Position, "The value of the position attribute can't be less than 0");
				}

				if (dictionary.ContainsKey(positionAttribute.Position))
				{
					throw ExceptionManager.GetIrrationalAttributeValue<PositionAttribute>
						(typeof(T), positionAttribute.Position, $"There is already a member that contains this value ({dictionary[positionAttribute.Position]})");
				}

				dictionary[positionAttribute.Position] = decoratorModule;
			}
		}

		private static IEnumerable<MemberInfo> DiscoverMembers()
		{
			var discoverAttributes = typeof(T)
										.GetCustomAttributes(true)
										.OfType<DiscoverAttribute>();

			if (discoverAttributes.Count() > 0)
			{
				// there are discover attributes
				// we will ONLY discover what they have specified in the discover attributes

				return discoverAttributes
							.SelectMany(x => typeof(T).GetMembersRecursively(x.BindingFlags))
							.Where(x => x.GetCustomAttributes(true)
											.OfType<PositionAttribute>()
											.Count() > 0);
			}
			else
			{
				// no DiscoverAttribute?
				// we will just search for all public and instance ones then

				return typeof(T).GetMembersRecursively(BindingFlags.Public | BindingFlags.Instance)
							.Where(x => x.GetCustomAttributes(true)
											.OfType<PositionAttribute>()
											.Count() > 0);
			}
		}

		private static IModuleAttribute GetPairingOf(MemberInfo member)
		{
			var attributes = member.GetCustomAttributes()
									.OfType<IModuleAttribute>();

			var attributesCount = attributes.Count();

			if (attributesCount < 1)
			{
				throw ExceptionManager.GetBrokenAttributePairing<PositionAttribute>
					(member.DeclaringType, member.Name, $"//TODO: FIX ME");
			}
			else if (attributesCount > 1)
			{
				throw ExceptionManager.GetIrrationalAttribute
					("There are more modifiers then necessary, try removing a few.");
			}

			return attributes.First();
		}
	}
}