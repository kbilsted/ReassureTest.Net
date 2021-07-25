# ReassureTest
*Making testing fun, fast and easy...*

The main problems with traditional unit tests that we seek to eliminate are are


1. Writing tests is a laborious and boring task.
2. Asserts expressed as code yields poor readability full of noise.
3. Code and test easily gets out of sync.
4. Tests are detrimental to change.
5. Tests have poor convincibility of coverage.

These points are further elaborated at https://github.com/kbilsted/StatePrinter/blob/master/doc/TheProblemsWithTraditionalUnitTesting.md (will be ported to this repo...).

<br>

ReassureTest is:
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

We achieve these goals by using a novel new way of specifying asserts. Expected values are expressed using a domain specific language. And upon a mismatch, ReassureTest prints the actual values **effectively doing 99% of the assert-typing work for you, and making it a breeze to learn.** 


Works with:

* Nunit ✔️ 
* Xunit ✔️
* MS Test ✔️
* ... any other testing framework ✔️



<br/>

# 1. An example workflow

To write assert's, we've made it short and sweet with a `Is()` method. Let's put it to action for testing our new shopping basket implementation. 
In this example we use `nunit` for setting up and executing tests but **ReassureTest works with any testing framework**.

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

voulait! Almost all typing was spent on fleshing out the actions of the test (the short part of a test - or you are doing it wrong). The lenghty part is autogenerated.


#2. A closer look at the assert

Let's take a closer look at what exactly has been generated.

We see a *specification* has been generated, focusing on fields and their values. The language has been designed to be free of noise that inevitably follow with writing asserts as code or when using JSON for specifications. 
The language draws inspiration from JSON so it should look familiar to most technical people. 

The specification is essentially a bunch of asserts, e.g. we check the `TotalPrice` is correct, that a discount order row has been added etc.

There is a bit of *lazy matching* going on as well. 

First we see the values `Id = guid-0`, `Id = guid-1` etc. These refer to actual guid values. Typically, what we think important about guid is that e.g. orderlines order id is the actual id of the order - but we care less about the value. Of course, you may configure ReasureTest to use exact guid values if you prefer. 

Let's turn our attention to `OrderDate = now`. Since we store the order date as "now" ReasureTest uses `now` as a value rather than an actual date. All date comparison has an allowed slack, meaning that if the orderdate is slightly the value of "now" we allow `now` to be the expected value, and when we re-run our test, as long as the order date is within our allowed slack, we assert the two dates to be equal. The datetime slack is configurable.

Finally, strings are surrounded by <code>`</code> rather than `"`. This is quite deliberate, since it allows us to copy-paste the output of ReasureTest into a C# string without the need for escape charaters. Ultimately, making it much easier to deal with, read and write.



# 3. Comparison with traditional asserts

<br/>
<br/>

# 3. Scope

ReasureTest's focus primarily on automated api tests, integration tests and component tests - as depicted in "the testing pyramid". You can use it for unit tests as well, when you want to combine expected values.

<img src="docs/devopsgroup_testing_pyramids_ideal_001.svg" width="40%">

A lot of litterature on why moving up the "test pyramid" may be beneficial to you can be found here:

* "Why Most Unit Testing is Waste" by James O Coplien https://rbcs-us.com/documents/Why-Most-Unit-Testing-is-Waste.pdf 
* "Unit Testing is Overrated" by Alexey Golub  https://tyrrrz.me/blog/unit-testing-is-overrated



<br/>
<br/>

# 4. Making asserts robust with fuzzy matching 

  * We assert that the rubber duck's `SKU` is a non-null value using `*`.
  * We use `*` as a wild card in string matching
  * We use `near now` to leniently assert the order is created within the last few seconds




```csharp
public void When_checking_out_product_Then_get_discount()
{
    var basket = new shoppingBasket();
    basket.Add(3, "rubber duck special");
    var order = basket.Checkout();

    order.Is(@"
    {
        CreatedDate = now 
        TotalPrice = 55.66
        OrderLines = [
          {
              Count = 3
              SKU = *
              Title = `rubber duck *`
              Amount = 20.61
          },
          {
              Count = 1
              SKU = null
              Title = `Spring sale 10%`
              Amount = 6.183
          } 
        ] 
    }");
}
```


<br/>
<br/>

# 5. Fuzzy matching rules

At times it is convenient to assert values leniently rather than strictly. It enables us to write asserts that are "good enough" to be valuable but without requiring complete control over all dependences.

### Values
* `?`: `null` or any value
* `*`: any non-null value


### Array elements
* ``**``: zero or more elements
* `*`: any element


### Dates
* __`today`: Today's date'__
* `now`: DateTime now


### Strings
* `*`: zero or more characters


### Guids
* `guid-x` represents a unique guid value, without specifying the exact value. This is used for ensuring two or more guids are the same or different.


<br/>
<br/>

# 5. Advanced topics

### Custom type handling

### fine tuning 




This project is an evolution of StatePrinter (https://github.com/kbilsted/StatePrinter/).


Have fun
-Kasper B. Graversen