namespace Decorator.ModuleAPI
{
	public interface IDecoratorDecorableModuleBuilder
	{
		DecoratorModule<T> BuildDecorable<T>(ModuleContainer modContainer)
			where T : IDecorable, new();
	}
}