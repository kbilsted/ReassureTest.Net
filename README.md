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
    basket.Add(3, "rubber duck special");
    var order = basket.Checkout();

    order.Is(@"");
}
```

Line 7 states the order is empty (`order.Is(@"")`). Clearly this is incorrect! But worry not, we want the framework to do some heavy lifting for us. When running the test case, it fails with the message

```
Expected: ""
But was:  "{ CreatedDate = 2021-06-..."
Actual is:
  {
      CreatedDate = 2021-06-21T14:53:22
      TotalPrice = 55.66
      OrderLines = [
        {
            Count = 3
            SKU = 1231234
            Title = `rubber duck special`
            Amount = 20.61
        },
        {
            Count = 1
            SKU = null
            Title = `Spring sale 10%`
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
    basket.Add(3, "rubber duck special");
    var order = basket.Checkout();

    order.Is(@" {
      CreatedDate = 2021-06-21T14:53:22
      TotalPrice = 55.66
      OrderLines = [
        {
            Count = 3
            SKU = 1231234
            Title = `rubber duck special`
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

We make the following observations along the way

  * Asserts are written in a simple language that is readable and free of noise. Many will recognize its resemblance to JSON
  * We perform more than one assert in our test - we check the `TotalPrice` is calculation, and that the seasonal discount is added as an order item.
  * We see strings are specified using ``` rather than `"` to make them easy to copy around.



<br/>
<br/>

# 2. Scope

ReasureTest focus primarily on integration tests 

<img src="docs/devopsgroup_testing_pyramids_ideal_001.svg" width="40%">

A lot of litterature on why this may be beneficial to you can be found here

* "Why Most Unit Testing is Waste" by James O Coplien https://rbcs-us.com/documents/Why-Most-Unit-Testing-is-Waste.pdf 
* "Unit Testing is Overrated" by Alexey Golub  https://tyrrrz.me/blog/unit-testing-is-overrated



<br/>
<br/>

# 3. Making asserts robust with fuzzy matching 

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

# 4. Fuzzy matching rules

At times it is convenient to assert values leniently rather than strictly. It enables us to write asserts that are "good enough" to be valuable but without requiring complete control over all dependences.

### Values
* `?`: `null` or any value
* `*`: any non-null value


### array
* ``**``: zero or more elements
* `*`: any element


### dates
* `today`: Today's date'
* `now`: DateTime now


### string
* `*`: zero or more characters



<br/>
<br/>

# 5. Advanced topics

### Custom type handling

### fine tuning 
