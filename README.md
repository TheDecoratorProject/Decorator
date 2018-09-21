# Decorator ![Nuget Version](https://img.shields.io/nuget/v/SirJosh3917.Decorator.svg) ![Nuget Downloads](https://img.shields.io/nuget/dt/SirJosh3917.Decorator.svg) [![Build status](https://ci.appveyor.com/api/projects/status/6ooio3rqlsbhs1q2?svg=true)](https://ci.appveyor.com/project/SirJosh3917/decorator) [![codecov](https://codecov.io/gh/SirJosh3917/Decorator/branch/master/graph/badge.svg)](https://codecov.io/gh/SirJosh3917/Decorator) [![Codacy Badge](https://api.codacy.com/project/badge/Grade/43061e7f10a04bfd8dd91f185fc1303a)](https://www.codacy.com/app/SirJosh3917/Decorator?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=SirJosh3917/Decorator&amp;utm_campaign=Badge_Grade)
Decorate classes with attributes and parse an array of objects and a string type into a class - forget about handling bad input forever.

```
PM> Package-Install SirJosh3917.Decorator
```

`Decorator` was taken :(

# Why?
Decorator is useful for taking an `object[]` array and converting it into a class - it ensures type safety and prevents malformed user input from ever happening.

# Show-me-the-code Tutorial
First, we'll create our message type

```cs
[Message("ping")]
public class PingMessage {

    [Position(0), Required]
    public DateTime ReplyTime { get; set; }
}
```

Then, let's deserialize an `object[]` to this class.

```cs
object[] array = new object[] { DateTime.UtcNow }

PingMessage deserialized = Deserializer.Deserialize<PingMessage>(new BasicMessage("ping", array));

Assert.Equal(array[0], deserialized.ReplyTime);
```

## Deserializing to methods
Let's say you want to magically deserialize a message to a method, well this would be how you do it!

```cs
[Message("a")]
public class MessageA {
    [Position(0), Required]
    public string Value { get; set; }
}

[Message("b")]
public class MessageB {
    [Position(0), Required]
    public string Value { get; set; }
}

public class HandleIncomingMessages {
    [DeserializedHandler]
    public void HandleMessageAs(MessageA msg) {
        Console.WriteLine($"A - {msg.Value}");
    }
    
    [DeserializedHandler]
    public void HandleMessageBs(MessageB msg) {
        Console.WriteLine($"B - {msg.Value}");
    }
}

HandleIncomingMessages instance = new HandleIncomingMessages();

Deserializer<HandleIncomingMessages>.DeserializeMessageToMethod(instance, new BasicMessage("b", "Hello, "));
Deserializer<HandleIncomingMessages>.DeserializeMessageToMethod(instance, new BasicMessage("a", "World!"));

//At the moment, static isn't handled properly
```

## Deserializing multiple messages in one message to a method
If you want to be able to recieve multiple of the same message, throw a `Repeatable` attribute to recieve multiple of the message.

```cs
[Message("chat"), Repeatable]
public class Chat {
    [Position(0), Required]
    public string Message { get; set; }
}

IEnumerable<Chat> chats = Deserializer.DeserializeRepeats<Chat>(new BasicMessage("chat", "These", "Are", "Multiple", "Messages!"));
foreach(var i in chats) {
    Console.WriteLine(i.Message);
}
```

## Serializing stuff
Serializing is easier then deserialization, and it's simple enough.

```cs
[Message("chat"), Repeatable]
public class Chat {
    [Position(0), Required]
    public string Message { get; set; }
}

BaseMessage msg = Serializer.Serialize<Chat>(new Chat {
    Message = "Test!"
});

// msg.Arguments is an object[] { "Test!" }

// you can also serialize multiple
BaseMessage msg = Serializer.Serialize<Chat>(new Chat[] {
    new Chat { Message = "Test!" },
    new Chat { Message = "Multiple" },
    new Chat { Message = "Things!" },
});

// msg.Arguments is an object[] { "Test!", "Multiple", "Things!" }
```

## Requiredness

You may have seen the `[Required]` attribute on our properties. This is behaviour by default, and you don't need to speicfy it.
There *is* an `[Optional]` attribute though, and you can use it to say that a message property doesn't *have* to be set.

```cs
[Message("test")]
public class Example {
    [Position(0), Optional]
    public string Value { get; set; }
}

Example exmp = Deserializer.Deserialize<Example>(new BasicMessage("test"));

//the above line of code will throw an exception, the message count must be 1

Example exmp = Deserializer.Deserialize<Example>(new BasicMessage("test", null));

//exmp.Value is 'default(string)', a.k.a. 'null'

Example exmp = Deserializer.Deserialize<Example>(new BasicMessage("test", 1929495));

//exmp.Value is 'default(string)', a.k.a. 'null'

Example exmp = Deserializer.Deserialize<Example>(new BasicMessage("test", "value"));

//exmp.Value is "value"
```

## Position

`[Position(x)]` specifies what position the property is in the `object[]` array.

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