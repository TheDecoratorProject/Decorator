# Decorator

Decorate classes with attributes and serialize object arrays into a concrete classes.

[![NuGet version][badge_nuget_version]][link_nuget]
[![NuGet downloads][badge_nuget_downloads]][link_nuget]
[![Build status][badge_appveyor_build]][link_appveyor_build]
[![AppVeyor tests][badge_appveyor_tests]][link_appveyor_build]
[![Codecov coverage][badge_codecov]][link_codecov_dashboard]
[![Codacy grade][badge_codacy_grade]][link_codacy_dashboard]

```
PM> Package-Install SirJosh3917.Decorator
dotnet add package SirJosh3917.Decorator
```

## Overview

```cs
public class User
{
	[Position(0), Required]
	public string Username { get; set; }

	[Position(1), Required]
	public int Id { get; set; }

	[Position(2), Required]
	public int Money { get; set; }
}

public override void Run()
{
	User myUser = new User
	{
		Username = "SirJosh3917",
		Id = 1337,
		Money = 1_000_000,
	};

	object[] serializedUser = DConverter<User>.Serialize(myUser);
	Console.WriteLine(JsonConvert.SerializeObject(serializedUser));
	// outputs:
	// ["SirJosh3917",1337,1000000]

	object[] userData = new object[] { "Jeremy", 63, 1_000 };

	if (DConverter<User>.TryDeserialize(userData, out User deserializedUser))
	{
		// this gets evaluated

		Console.WriteLine(JsonConvert.SerializeObject(deserializedUser));
		// outputs:
		// {"Username":"Jeremy","Id":63,"Money":1000}
	}
}
```

## Examples

See the [example project][link_examples] for examples.

## Dependancy Graph

![dependancy_graph]

### Legacy Decorator

[This was the repo on the final commit of the 1xx Decorator series.][link_final_1xx]

## License

[![MIT License (MIT)][badge_license]][link_license]

[badge_nuget_version]: https://img.shields.io/nuget/v/SirJosh3917.Decorator.svg
[badge_nuget_downloads]: https://img.shields.io/nuget/dt/SirJosh3917.Decorator.svg
[badge_appveyor_build]: https://ci.appveyor.com/api/projects/status/6ooio3rqlsbhs1q2?svg=true
[badge_appveyor_tests]: https://img.shields.io/appveyor/tests/SirJosh3917/Decorator/master.svg
[badge_codacy_grade]: https://api.codacy.com/project/badge/Grade/43061e7f10a04bfd8dd91f185fc1303a
[badge_codecov]: https://img.shields.io/codecov/c/github/SirJosh3917/Decorator.svg
[badge_license]: https://img.shields.io/github/license/SirJosh3917/Decorator.svg

[dependancy_graph]: ./DependancyGraph.png

[link_nuget]: https://www.nuget.org/packages/SirJosh3917.Decorator/
[link_appveyor_build]: https://ci.appveyor.com/project/SirJosh3917/Decorator
[link_codacy_dashboard]: https://app.codacy.com/project/SirJosh3917/Decorator/dashboard
[link_codecov_dashboard]: https://codecov.io/gh/SirJosh3917/Decorator
[link_license]: ./LICENSE

[link_examples]: ./Decorator.Examples/Examples/
[link_final_1xx]: https://github.com/SirJosh3917/Decorator/tree/bd3954577be2abbb2e358b1345d73c2d5e1ca4ae