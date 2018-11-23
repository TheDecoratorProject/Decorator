using Decorator.Attributes;
using Decorator.Exceptions;
using Decorator.ModuleAPI;
using Decorator.ModuleAPI.IL;

using StrictEmit;

using SwissILKnife;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Decorator.Compiler
{
	// [CLEAN]
	// TODO: Clean
	// TODO: Comment on what the IL so i can actually read it

	public delegate bool ILDeserialize<T>(object[] array, ref int i, out T result);

	public delegate object[] ILSerialize<T>(T item);

	public class Compiler<T> : ICompiler<T>
		where T : new()
	{
		public BaseModule[] Compile(Func<MemberInfo, BaseContainer> getContainer)
		{
			// cache constructor
			InstanceOf<T>.Create();

			// store all the props/fields that we need
			var dict = new SortedDictionary<int, BaseModule>();
			SetDecoratorModules(dict, DiscoverMembers(), getContainer);

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

		public bool SupportsIL(BaseModule[] modules)
		{
			// for every module
			foreach (var i in modules)
			{
				// if it doesn't have ILSupport
				if (!i.GetType()
					.GetInterfaces()
					.Contains(typeof(ILSupport)))
				{
					return false;
				}
			}

			return true;
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

			// int objSize = 0;
			var objSize = il.DeclareLocal(typeof(int));
			il.EmitConstantInt(0);
			il.EmitSetLocalVariable(objSize);

			foreach (var i in GetILModules(modules))
			{
				i.GenerateEstimateSize(il, getEstimateSizeMethodContainer());
			}

			var objArray = il.DeclareLocal(typeof(object[]));
			var index = il.DeclareLocal(typeof(int));

			// int i = 0;
			il.EmitConstantInt(0);
			il.EmitSetLocalVariable(index);

			// object[] objArray = new object[objSize];
			il.EmitLoadLocalVariable(objSize);
			il.EmitNewArray(typeof(object));
			il.EmitSetLocalVariable(objArray);

			int c = 0;
			foreach (var i in GetILModules(modules))
			{
				i.GenerateSerialize(il, getSerializeMethodContainer());

				c++;
			}

			// return objArray;
			il.EmitLoadLocalVariable(objArray);
			il.EmitReturn();

			return dm.CreateDelegate();

			void emitLoadSerializingItem() => il.EmitLoadArgument(0);

			ILEstimateSizeMethodContainer getEstimateSizeMethodContainer() => new ILEstimateSizeMethodContainer(
					() =>
					{
						emitLoadSerializingItem();
					},
					(load) =>
					{
						load();
						il.EmitLoadLocalVariable(objSize);
						il.EmitAdd();
						il.EmitSetLocalVariable(objSize);
					}
				);

			ILSerializeMethodContainer getSerializeMethodContainer() => new ILSerializeMethodContainer(
					index,
					() =>
					{
						MemberUtils.EmitILForGetMethod(il, modules[c].ModuleContainer.Member.GetMember, () =>
						{
							emitLoadSerializingItem();
						});
					},
					(emit) =>
					{
						il.EmitLoadLocalVariable(objArray);
						il.EmitLoadLocalVariable(index);
						emit();
						il.EmitSetArrayElement<object>();
					}
				);
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

			// result = new T();
			emitLoadResultArg();
			il.EmitNewObject<T>();
			il.EmitSet(typeof(T));

			int c = 0;
			foreach (var member in GetILModules(modules))
			{
				var module = modules[c];
				var returnIfLessLabel = il.DefineLabel();

				// if (objArray.Length < i) {
				emitLoadObjectArrayArg();
				il.EmitLoadArrayLength();
				il.EmitConvertToInt();

				emitLoadIndexArg();
				il.EmitLoad<int>();

				il.EmitShortBranchGreaterThan(returnIfLessLabel); // <

				// return false;
				emitReturnBool(false);

				// }
				il.MarkLabel(returnIfLessLabel);

				// emit whatever code is in that part of IL
				member.GenerateDeserialize(il, getDeserializeMethodContainer(module));

				c++;
			}

			// return true;
			emitReturnBool(true);

			return dm.CreateDelegate();

			// il helpers
			void emitLoadObjectArrayArg() => il.EmitLoadArgument(0);
			void emitLoadIndexArg() => il.EmitLoadArgument(1);
			void emitLoadResultArg() => il.EmitLoadArgument(2);

			void emitIndexValue()
			{
				emitLoadIndexArg();
				il.EmitLoad<int>();
			}

			void emitReturnBool(bool returnResult)
			{
				const int trueValue = 1;
				const int falseValue = 0;
				il.EmitConstantInt(returnResult ? trueValue : falseValue);

				il.EmitReturn();
			}

			ILDeserializeMethodContainer getDeserializeMethodContainer(BaseModule module)
			{
				return new ILDeserializeMethodContainer(
					loadMemberValue: () =>
					{
						// pushes result.Property onto the stack
						il.EmitILForGetMethod(module.ModuleContainer.Member.GetMember,
						() =>
						{
							emitLoadResultArg();
						});
					},
					setMemberValue: (emitValueToSetTo) =>
					{
						// result.Property = <whatever v() pushes>;
						il.EmitILForSetMethod(module.ModuleContainer.Member.GetMember,
							() =>
							{
								emitLoadResultArg();
							},
							() =>
							{
								il.EmitLoad(typeof(object));
								emitValueToSetTo();
							});
					},
					loadCurrentObject: () =>
					{
						// objArray[i];
						emitLoadObjectArrayArg();
						emitIndexValue();
						il.EmitLoadArrayElement<object>();
					},
					loadIndex: () =>
					{
						// i;
						emitIndexValue();
					},
					addToIndex: (emitAmountToAdd) =>
					{
						// i += a;
						emitLoadIndexArg();
						emitIndexValue();
						emitAmountToAdd();
						il.EmitAdd();
						il.EmitSet(typeof(int));
					}
				);
			}
		}

		private static IEnumerable<ILSupport> GetILModules(BaseModule[] modules)
		{
			foreach (var i in modules)
			{
				if (i is ILSupport ilSupport)
				{
					yield return ilSupport;
					continue;
				}

				throw new DecoratorCompilerException($"IL generation is not supported on this type.", null);
			};
		}
	}
}