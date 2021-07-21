# ReassureTest
*Making testing fun, fast and easy...*

Why should you use ReassureTest?

  * **Simpler to read tests:** We've removed the noise that makes test feel like code, rather than feel like a specification
  * **Simpler to write:** Let the framework do most of the typing for you , write and maintain asserts for object graphs
  * **Simpler to maintain:** ReasureTest automatically detect when your types change (e.g. new fields) and generates new asserts for you

We achieve these goals by using a novel new way of specifying asserts. Expected values are expressed using a domain specific language. And upon a mismatch, ReassureTest prints the actual values **effectively doing 99% of the typing work for you and making it a breeze to learn.** 

The simple language makes it possible to share and discuss actual tests with human testers, and to some degree, enable testers to write the expected values themselves!


<br/>

# 1. An example workflow

To assert some value, we've made it short and sweet using a new `Is` method. Let's put it to action for testing our new shopping basket implementation. 
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

Line 7 states the order is empty (`""`). Clearly this is incorrect! But worry not, we want the framework to do some heavy lifting for us. When running the test case, it fails with a message

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

We now copy-paste this into our test

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

![Testing pyramid from ](docs/devopsgroup_testing_pyramids_ideal_001.svg?raw=true "Testing pyramid")

A lot of litterature on why this may be beneficial to you can be found here

* "Why Most Unit Testing is Waste" by James O Coplien https://rbcs-us.com/documents/Why-Most-Unit-Testing-is-Waste.pdf 
* "Unit Testing is Overrated" by Alexey Golub  https://tyrrrz.me/blog/unit-testing-is-overrated



<br/>
<br/>

# 3. Makeing the assert robust with fuzzy matching 

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
