# ReassureTest
Automate unit testing with code generation and DSL

The goals are

  * Simpler to read, write and maintain asserts for object graphs
  * Automatic detection when fields are added to objects

We achieve these goals by using a domain specific language for specifying expected values and by allowing Reassure.net to do the heavy lifting for you. It can

## Example

To assert some value, we've made it short and sweet using a new `Is` method.

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

A number of observations can be made from this example
  * Asserts are written in a simple format closely resembling JSON
  * We perform more than one assert in our test - we check the `TotalPrice` is calculation, and that the seasonal discount is added as an order item.
  * We see strings are specified using ``` rather than `"`.
  * We assert that the rubber duck's `SKU` is a non-null value using `*`.
  * We use `*` as a wild card in string matching
  * We use `near now` to leniently assert the order is created within the last few seconds

## Auto generated asserts

Whenever there is a difference between the expected and actual values, ReassureTest outputs the actual values either 
to the screen or modifies your test for your convenience.

For example, rather than typing the assertion above, simply type

```csharp
order.Is();
```

And ReassureTest prints the string for you to copy



## Fuzzy  matching

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


## integrating with unit test frameworks



# Advanced topics

## Custom type handling

## fine tuning 
