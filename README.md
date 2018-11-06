# ATTENTION!

Decorator is undergoing a major change, from 1.x.x to 2.x.x

The current readme is for Decorator 1.x.x, please view the entire repo in 1.x.x mode.

[Please view the entire repo in 1.x.x mode](https://github.com/SirJosh3917/Decorator/tree/bd3954577be2abbb2e358b1345d73c2d5e1ca4ae)

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

## Deserialization

Create a message type.

```cs
[Message("ping")]
public class PingMessage {

    [Position(0), Required]
    public DateTime ReplyTime { get; set; }
}
```

Deserialize an `object[]` to this class.

```cs
DateTime time = DateTime.UtcNow;
object[] array = new object[] { time }

PingMessage deserialized = null;

Deserializer.TryDeserializeItem<PingMessage>(new BasicMessage("ping", array), out deserialized);
// 1.2.0-pre: Deserializer<PingMessage>.TryDeserializeItem(new BasicMessage("ping", array), out deserialized);

// deserialized.ReplyTime contains now the value from time
```

## Serialization

### For a single class instance

Create a message type.
```cs
[Message("chat"), Repeatable]
public class Chat {
    [Position(0), Required]
    public string Message { get; set; }
}
```


Serialize into a BaseMessage. ``BaseMessage.Arguments`` is an ``object[]``. Its content would be ``object[] {"Test!"}``.

```cs
BaseMessage msg = Serializer.SerializeItem<Chat>(new Chat {
    Message = "Test!"
});
```

### For multiple class instances of the same type

Create an Enumerable of messages.
```cs
var messages = new Chat[] {
    new Chat { Message = "Test!" },
    new Chat { Message = "Multiple" },
    new Chat { Message = "Things!" },
}
```

Serialize into a BaseMessage. ``BaseMessage.Arguments`` is an ``object[]``. Its content would be ``object[] { "Test!", "Multiple", "Things!" }``

```cs
BaseMessage msg = Serializer.SerializeItems<Chat>(messages);
```

## Optional

The `[Optional]` attribute, can be used to mark a property nonobligatory.

```cs
[Message("test")]
public class Example {
    [Position(0), Optional]
    public string Value { get; set; }
}

Example exmp = null;

//1.2.0-pre note: Replace Deserializer.TryDeserializeItem<Example> with Deserializer<Example>.TryDeserializeItem

Deserializer.TryDeserializeItem<Example>(new BasicMessage("test"), out exmp);
// the above line of code will throw an exception, the message count must be 1

Deserializer.TryDeserializeItem<Example>(new BasicMessage("test", null), out exmp);
// exmp.Value is 'default(string)', a.k.a. 'null'

Deserializer.TryDeserializeItem<Example>(new BasicMessage("test", 1929495), out exmp);
// exmp.Value is 'default(string)', a.k.a. 'null'

Deserializer.TryDeserializeItem<Example>(new BasicMessage("test", "value"), out exmp);
// exmp.Value is "value"
```

## Position

`[Position(x)]` specifies which index in the `object[]` holds the corresponding value.

```cs
[Message("example")]
public class Example {
    [Position(3)]
    public string MyValue { get; set; }
    
    [Position(1)]
    public int TestInt { get; set; }
    
    [Position(2)]
    public DateTime Date { get; set; }
    
    [Position(0)]
    public short ShortValue { get; set; }
}

BaseMessage bm = Serializer.Serialize<Example>(new Example {
    MyValue = "string",
    TestInt = 1337,
    Date = DateTime.UtcNow,
    ShortValue = short.MaxValue
});

// bm.Arguments = new object[] { short.MaxValue, 1337, DateTime.UtcNow, "string" };
```

## License


[![MIT License (MIT)][badge_license]][link_license]

[badge_nuget_version]: https://img.shields.io/nuget/v/SirJosh3917.Decorator.svg
[badge_nuget_downloads]: https://img.shields.io/nuget/dt/SirJosh3917.Decorator.svg
[badge_appveyor_build]: https://ci.appveyor.com/api/projects/status/6ooio3rqlsbhs1q2?svg=true
[badge_appveyor_tests]: https://img.shields.io/appveyor/tests/SirJosh3917/Decorator/master.svg
[badge_codacy_grade]: https://api.codacy.com/project/badge/Grade/43061e7f10a04bfd8dd91f185fc1303a
[badge_codecov]: https://img.shields.io/codecov/c/github/SirJosh3917/Decorator.svg
[badge_license]: https://img.shields.io/github/license/SirJosh3917/Decorator.svg

[link_nuget]: https://www.nuget.org/packages/SirJosh3917.Decorator/
[link_appveyor_build]: https://ci.appveyor.com/project/SirJosh3917/Decorator
[link_codacy_dashboard]: https://app.codacy.com/project/SirJosh3917/Decorator/dashboard
[link_codecov_dashboard]: https://codecov.io/gh/SirJosh3917/Decorator
[link_license]: ./LICENSE