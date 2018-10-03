# Decorator

Decorate classes with attributes and serialize object arrays into a concrete classes.

![Nuget Version](https://img.shields.io/nuget/v/SirJosh3917.Decorator.svg) ![Nuget Downloads](https://img.shields.io/nuget/dt/SirJosh3917.Decorator.svg) 

[![Build status](https://ci.appveyor.com/api/projects/status/6ooio3rqlsbhs1q2?svg=true)](https://ci.appveyor.com/project/SirJosh3917/decorator) [![codecov](https://codecov.io/gh/SirJosh3917/Decorator/branch/master/graph/badge.svg)](https://codecov.io/gh/SirJosh3917/Decorator) [![Codacy Badge](https://api.codacy.com/project/badge/Grade/43061e7f10a04bfd8dd91f185fc1303a)](https://www.codacy.com/app/SirJosh3917/Decorator?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=SirJosh3917/Decorator&amp;utm_campaign=Badge_Grade)

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
dateTime time = DateTime.UtcNow;
object[] array = new object[] { time }

PingMessage deserialized = null;

Deserializer.TryDeserializeItem<PingMessage>(new BasicMessage("ping", array), out deserialized);
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

[MIT License (MIT)](./LICENSE)
