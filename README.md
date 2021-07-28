# ReassureTest
*Making testing fun, fast and easy...*

<br>

<table>
    <tr>
        <td align="center" valign="center">
            <img src="docs/2849813_multimedia_eyeglasses_glasses_spectacles_icon.svg" width="80">
            <br><b>Intention revealing tests</b>
            <br><i>Fuzzy matching rules combined with a simple assert language makes your tests smaller and concise.</i></td>
        <td align="center" valign="center">
            <img src="docs/pen.svg" width="80">
            <br><b>Faster to write</b>
            <br><i>Asserts are expressed with much less typing, and you can have ReassureTest do most of the typing for you!</i></td>
    </tr>
    <tr>
        <td align="center" valign="center">
            <img src="docs/pulse.svg" width="80"> 
            <br><b>Simpler to maintain</b>
            <br><i>ReasureTest automatically detects changes in your code base (e.g. new fields) and generates new asserts for you.</i></td>
        <td align="center" valign="center">
            <img src="docs/2849830_multimedia_options_setting_settings_gear_icon.svg" width="80"> 
            <br><b>Highly configurable</b>
            <br><i>We provide enough flexibility to cater for your needs to make complex types simple to represent and assert.</i></td>
    </tr>
    <tr>
        <td align="center" valign="center">
            <img src="docs/16071409381537184100-128.png" width="80">
            <br><b>Asserts as specifications</b>
            <br><i>Assert are easier to share, discuss and edit with non-technical people, and enable testers to write the expected values themselves.</i></td>
        <td align="center" valign="center">
            <img src="docs/10863499491535956127-128.png" width="80"> 
            <br><b>Free & Open source</b>
            <br><i>Respects your freedom to run it, to study and change it, and to redistribute copies with or without changes.</i></td>
    </tr>
</table>

<br>
<br>

**Why use ReassureTest**

The main problems with traditional unit tests that we seek to eliminate are are

1. Writing tests is a laborious and boring task.
2. Asserts expressed as code yields poor readability full of noise.
3. Code and test easily gets out of sync.
4. Tests are detrimental to change.
5. Tests have poor convincibility of coverage.

These points are further elaborated at https://github.com/kbilsted/StatePrinter/blob/master/doc/TheProblemsWithTraditionalUnitTesting.md (will be ported to this repo...).


We achieve these goals by using a novel new way of specifying asserts. Expected values are expressed using a domain specific language. And upon a mismatch, ReassureTest prints the actual values **effectively doing 99% of the assert-typing work for you, and making it a breeze to learn.** 


<br>

**Compatibility**

<table>
    <tr>
        <th>Supported testing framework</th>
        <th>Supported .Net version</th>
    </tr>
    <tr>
        <td>
            <ul>
                <li> Nunit ✔️ 
                <li> Xunit ✔️
                <li> MS Test ✔️
                <li> ... any other testing framework ✔️
            </ul>
        </td>
        <td>
            <ul>
                <li> .Net framework 4.5 ✔️ 
                <li> .Net standard 2.0 ✔️ 
                <li> .Net Core 3.1 ✔️ 
            </ul>
        </td>
    </tr>
</table>

<br/>
<br/>

# 1. Getting started 

1. Install the nuget package `ReassureTest` from nuget.org (`dotnet add package ReassureTest`)
2. Use the `Is()` method in your tests
3. Done




<br/>
<br/>

# 2. An example workflow

Asserts are expressed using an `Is()` method. Let's put it to action for testing a shopping basket implementation. 

In this example we use Nunit for setting up and executing tests but **ReassureTest works with any testing framework**.

```csharp
[Test]
public void When_ordering_rubber_ducs_Then_get_a_discount()
{
    var basket = new shoppingBasket();
    basket.LatestDeliveryDate = new DateTime(2020, 02, 01);
    basket.Add(3, "Rubber duck special");
    var order = basket.Checkout();

    order.Is(@"");
}
```

Line 7 states the order is "nothing": (`order.Is(@"")`). Clearly this is incorrect! But worry not, we want the framework to do some heavy lifting for us. When running the test case, it fails with the message

```
Expected: ""
But was:  "{ Id = ..."
Actual is:
{ 
    Id = guid-0
    OrderDate = now
    LatestDeliveryDate = 2020-02-01T00:00:00
    TotalPrice = 26.98
    OrderLines = [
        {
            Id = guid-1
            Count = 3
            OrderId = guid-0
            Name = `Rubber duck special`
            SKU = `RD17930827`
            Amount = 9.99
        },
        {
            Id = guid-2
            Count = 1
            OrderId = guid-0
            Name = `Sale 10%`
            SKU = null
            Amount = 6.183
        }
    ]
}
```

We can now copy-paste this into our test

```csharp
[Test]
public void When_ordering_rubber_ducs_Then_get_a_discount()
{
    var basket = new shoppingBasket();
    basket.LatestDeliveryDate = new DateTime(2020, 02, 01);
    basket.Add(3, "Rubber duck special");
    var order = basket.Checkout();

    order.Is(@" {
        Id = guid-0
        OrderDate = now
        LatestDeliveryDate = 2020-02-01T00:00:00
        TotalPrice = 26.98
        OrderLines = [
            {
                Id = guid-1
                Count = 3
                OrderId = guid-0
                Name = `Rubber duck special`
                SKU = `RD17930827`
                Amount = 9.99
            },
            {
                Id = guid-2
                Count = 1
                OrderId = guid-0
                Name = `Sale 10%`
                SKU = null
                Amount = 6.183
            }
        ]
  }");
}
```

Done! We typed only the test-setup (the short part of a test). The lenghty part is autogenerated.


<br/>
<br/>

# 3. A closer look at the assert

Let's take a closer look at what exactly has been generated.

We see a *specification* has been generated, focusing on fields and their values. The language has been designed to be free of noise that inevitably follow with writing asserts as code or when using JSON for specifications. The language draws inspiration from JSON so it should look familiar to most technical people. 

The specification is essentially a bunch of asserts, e.g. we check the `TotalPrice` is correct, that a discount order row has been added etc.

ReassureTest uses configurable *fuzzy matching* to make asserts easier to read, write and maintain.

* `Id = guid-0`, `Id = guid-1`, ... refer to actual guid values. Typically, what we think important about guid is that e.g. orderlines order id is the actual id of the order - but we care less about the value. 
* `OrderDate = now` refer to the current clock rather than a set date. All date comparisons has an allowed slack, meaning that if the orderdate is slightly the value of "now" we allow `now` to be the expected value. 
* Strings are surrounded by <code>&#96;&#96;</code> rather than `""`. This is quite deliberate, as it becomes easy tto copy-paste the output of ReasureTest into a C# string without the need for escape charaters. Ultimately, making it much easier to deal with.



<br/>
<br/>

# 4. Comparison with traditional asserts

To put things into perspective, here is a side-by-side comparison with a traditional Nunit test using `Assert.AreEqual()`.

*There are actually two errors in the below listing, can you spot them? Not easy, right!*
<table>
<tr>
<td valign="top">

```csharp
var slack = TimeSpan.FromSeconds(2);
Assert.IsNotNull(order);
Assert.AreNotEqual(order.Id, Guid.Empty);
Assert.IsTrue((order.OrderDate-DateTime.Now).Duration() <= slack);
Assert.AreEqual(order.LatestDeliveryDate, basket.LatestDeliveryDate);
Assert.AreEqual(order.TotalPrice, 26.98M);
Assert.AreEqual(order.OrderLines.Count, 2);
Assert.AreNotEqual(order.OrderLines[0].Id, order.Id);
Assert.AreEqual(order.OrderLines[0].Count, 3);
Assert.AreEqual(order.OrderLines[0].OrderId, order.Id);
Assert.AreEqual(order.OrderLines[0].Name, "Rubber duck special");
Assert.AreEqual(order.OrderLines[0].SKU, "RD17930827");
Assert.AreEqual(order.OrderLines[0].Amount, 9.99);
Assert.AreNotEqual(order.OrderLines[1].Id, order.Id);
Assert.AreEqual(order.OrderLines[1].OrderId, order.Id);
Assert.AreEqual(order.OrderLines[1].Name, "Sale 10%");
Assert.AreEqual(order.OrderLines[1].SKU, null);
Assert.AreEqual(order.OrderLines[1].Amount, 2.997);
```
</td>
<td valign="top">

```csharp
order.Is(@" {
    Id = guid-0
    OrderDate = now
    LatestDeliveryDate = 2020-02-01T00:00:00
    TotalPrice = 26.98
    OrderLines = [
        {
            Id = guid-1
            Count = 3
            OrderId = guid-0
            Name = `Rubber duck special`
            SKU = `RD17930827`
            Amount = 9.99
        },
        {
            Id = guid-2
            Count = 1
            OrderId = guid-0
            Name = `Sale 10%`
            SKU = null
            Amount = 6.183
        }
    ]
}");
```
</td>
</tr>
    <tr>
        <td>Roughly 970 characters typed</td>
        <td>0 charecters typed (autogenerated)</td>
    </tr>
    <tr>
        <td>When the code changes: Manual task to identify need and update the test</td>
        <td>Automatically updated</td>
    </tr>
    <tr>
        <td>Are all fields covered: Manual task to identify</td>
        <td>Automatic</td>
    </tr>
    <tr>
        <td>Asserts as code, difficult to share and edit for non-programmers</td>
        <td>Asserts are expressed in a light-weight understandable format</td>
    </tr>
    <tr>
        <td>Roughly 50% of the asserts is "noise" (e.g. words "Assert.AreEqual"</td>
        <td>Asserts are expressed as a specification</td>
    </tr>
</table>

<br/>
<br/>



# 5. Fuzzy matching rules

It is often very convenient to assert values non-strictly. We call this *"fuzzy matching"*, and it enables us to write asserts that are "good enough" to be valuable while at the same time making the testing much easier. Either since we can require less than complete control over all dependences or because the asserts are easier to express. 

This is particular useful when you "move up the unit test pyramid". 

Here we explain what ReassureTest's language support in terms of fuzzy matching.


### Values
* `?`: `null` or any value
* `*`: any non-null value

e.g. `SKU = *` means that we want the SKU to have any non-null value. It can also be used on complex values such as `OrderLines = *`.


### Array elements
* ``**``: zero or more elements (not implemented)
* `*`: any element 

e.g.  `OrderLines = [ *, * ]` means that there are two order lines objects, both not null. 


### Dates
* `today`: Today's date' (not implemented)
* `now`: DateTime now
* Dates are equal when difference is less than date slack


### Decimal, float, double
* `decimal`, `double`, `float` are equal when difference is less than decimal slack (not implemented)


### Strings
* `*`: zero or more characters (not implemented)


### Guids
* `guid-x` represents a unique guid value, without specifying the exact value. This is used for ensuring two or more guids are the same or different.


### Exceptions
* Exceptions are transformed into a simple form, a class containing `Message`, `Data` and `Type`.

```csharp
var ex = new Exception("message") { Data = {{"a", "b"}} };

ex.Is(@"{
    Message = `message`
    Data = [
        {
            Key = `a`
            Value = `b`
        }
    ]
    Type = `System.Exception`
}");
```

<br/>
<br/>

# 6. Configuration
There are two ways you can configure ReassureTest

1. Use the global settings part of the api.
2. Use a configuration as a second parameter to `Is()`.

The first you use to change the overall characteristics of your usage, while the second is often to fit specific corner cases.

The default configuration can be changed by `Reassure.DefaultConfiguration`.

If you need a new copy of the default configuration you can use `var newCfg = Configuration.New()`.


## 6.1. Nunit example setup

For Nunit you can optionally setup a global setting using

```csharp
[SetUpFixture]
public class TestsSetup
{
    [OneTimeSetUp]
    public void Setup()
    {
        Reassure.DefaultConfiguration.Outputting.EnableDebugPrint = false;
        Reassure.DefaultConfiguration.TestFrameworkIntegration.RemapException = ex => new AssertionException(ex.Message, ex);
    }
}
```



<br/>
<br/>


# 7. Scope

ReasureTest's focus primarily on automated api tests, integration tests and component tests - as depicted in "the testing pyramid". You can use it for unit tests as well, when you want to combine expected values.

<img src="docs/devopsgroup_testing_pyramids_ideal_001.svg" width="40%">

A lot of litterature on why moving up the "test pyramid" may be beneficial to you. Here are some of the better productions

* "Why Most Unit Testing is Waste" by James O Coplien https://rbcs-us.com/documents/Why-Most-Unit-Testing-is-Waste.pdf 
* "Unit Testing is Overrated" by Alexey Golub  https://tyrrrz.me/blog/unit-testing-is-overrated


This project is an evolution of StatePrinter (https://github.com/kbilsted/StatePrinter/).


<br>
<br>
<br>
Have fun
<br>
 -Kasper B. Graversen
