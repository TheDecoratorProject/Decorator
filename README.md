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
```

# Why object[]?

Decorator was originally built for the purpose of taking loosely created, possibly incorrectly formulated `object[]`s and convert them into strictly typed classes and back.

# Any examples?

Yes, they're a WIP

You can browse the [work in progress example project][link_examples]

## Dependancy Graph

![dependancy_graph]

### Older Decorator

This is the final commit made to the Decorator 1.x.x series. If you'd like to view the repository in that mode, [feel free to do so][link_final_1xx].

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