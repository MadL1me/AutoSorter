# AutoSorter
Easy collection sorting on Attributes


## Example
### Old way
Imagine, you have CRM-like webapp with availability to sort through fields of customers.


Let's assume you have model like this: 

```cs
public class Customer
{
  public int Id { get; set; }
  public string Name { get; set; }
  public byte Age { get; set; }
  public bool IsDeleted { get; set; }
}
```

The basic implementation of data sorting will look smth like this:
 
```cs
public class CustomerController
{
  public async Task<List<Customer>> GetCustomers(int sortingValue, bool sortByDesc) 
  {
    var customers = await _db.GetCustomers();
  
    switch(sortingValue) 
    {
      case 0: sortByDesc ? customers.OrderByDescending(c => c.Id) : customers.OrderBy(c => c.Id); break;
      case 1: sortByDesc ? customers.OrderByDescending(c => c.Name) : customers.OrderBy(c => c.Name); break;
      case 2: sortByDesc ? customers.OrderByDescending(c => c.Age) : customers.OrderBy(c => c.Age); break;
      case 3: sortByDesc ? customers.OrderByDescending(c => c.IsDeleted) : customers.OrderBy(c => c.IsDeleted); break;
      // more redurant cases....
    }
    
    return customers;
  }
}
```

### New way
With AutoSorter there is new, simpler way to do sorting by fields logic. Firsty, assign attributes:

```cs
using AutoSorting.Attributes;

public class Customer
{
  [AutoSort(0)]
  public int Id { get; set; }
  [AutoSort(1)]
  public string Name { get; set; }
  [AutoSort(2)]
  public byte Age { get; set; }
  [AutoSort(3)]
  public bool IsDeleted { get; set; }
}
```

Now you can sort with one simple method call:

```cs
using AutoSorter;

public class CustomerController
{
  public async Task<List<Customer>> GetCustomers(int sortingValue, bool sortByDesc) 
  {
    var customers = await _db.GetCustomers();
    
    customers = AutoSorter.Sort(customers, sortingValue, sortByDesc); // That's all!
    
    return customers;
  }
}
```
