Xels FullNode
===============

https://www.xels.io/

Xels fullnode is a Hybrid PoW-PoS based system. It is developed on the [Stratis](https://stratisplatform.com/) platform. Stratis itself is a PoS based system. It also provides a PoW based implementation for Bitcoin Network.
We implemented a mix of PoW and PoS based system to garantee protect the network from 51% attack. For detail please check our website.  


Getting started - Building and running a Xels Full Node 
------------------

Our full node is currently in alpha.  

```

---------------

## Supported Platforms

* <b>Windows</b> - works from Windows 7 and later, on both x86 and x64 architecture. Most of the development and testing is happening here.
* <b>Linux</b> - works on Ubuntu 14.04 and later (x64).
* <b>MacOS</b> - works from OSX 10.12 and later. 

## Prerequisites

To install and run the node, you need
* [.NET Core 2.1](https://www.microsoft.com/net/download/core)
* [Git](https://git-scm.com/)

## Build instructions

### Get the repository and its dependencies

git clone https://github.com/xels-io/XelsFullNode.git  

dotnet build

```

### Build and run the code
With this node, you can connect to either the Xels network or the Bitcoin network, either on MainNet or TestNet.
So you have 4 options:

1. To run a <b>Xels</b> node on <b>MainNet</b>, do
```
cd Xels.XelsD
dotnet run
```  

2. To run a <b>Xels</b>  node on <b>TestNet</b>, do
```
cd Xels.XelsD
dotnet run -testnet
```  

3. To run a <b>Bitcoin</b> node on <b>MainNet</b>, do
```
cd Xels.BitcoinD
dotnet run
```  

4. To run a <b>Bitcoin</b> node on <b>TestNet</b>, do
```
cd Xels.BitcoinD
dotnet run -testnet
```  

### Advanced options

You can get a list of command line arguments to pass to the node with the -help command line argument. For example:
```
cd Xels.XelsD
dotnet run -help
```  

Testing Guidelines
===============

Unit Testing
------------
For unit testing we use Xunit and Moq.

Unit test preparations
1. If a class contains multiple classes try to see if you can move them to separate files inside the same folder. Ideally we have 1 file per class.
2. Create an interface for each class. This helps out a lot with testability. These can be inside the same file as the initial class or in a separate file in the same folder.
3. Try to use dependencies by their interface instead of by their concrete type as much as possible.
4. Move dependencies to the constructor so they can be moved into the dependency injection(DI) framework later. Again try to use their interface and not their concrete type.
   For backward compatibility an overload can be created until the DI registration is done.
  
  ```csharp
  public RepositoryUser()
  {
	this.Repository = new Repository();
  }
  ->
  public RepositoryUser() : this(new Repository());
  {	
  }  
  public RepositoryUser(IRepository repository)
  {
	Guard.NotNull(repository, nameof(repository));
	
	this.Repository = repository;
  }  
  ```
General project rules:
1. The unit test project has to be named {ProjectName}.Tests
2. The test file should be located at the same place in the test project. 
   For example if have file located in **Xels.Bitcoin\BlockStore\BlockRepository.cs** 
   it should be **Xels.Bitcoin.Test\BlockStore\BlockRepositoryTest.cs** in the test project.
3. As a file naming convention {ClassName}Test.cs

General testing rules:
1. Focus on testing only a single method at a time.
2. In you tests remove the dependencies from the class under test by using mocks. For more information regarding mocks look [HERE](https://github.com/Moq/moq4/wiki/Quickstart)
3. Do not try to test the entire method in one test. You may need more than 1 test to cover all possible cases.
4. The test method name follows the following structure: {MethodName}{GivenContext}{ExpectedOutcome}. Example for a method called Query: QueryWithoutInitializedRepositoryThrowsException.
5. Test public/Protected/Internal methods. Testing private methods should be an exceptional case.
6. A test must have the following structure(see example below):
	* Setup test context
	* NewLine
	* Call method under test
	* NewLine
	* Assert Result
7. DRY (don't repeat yourself). If you do the same initialization in each test move that to the test class constructor. This code is then called before every test.
8. Test getters and setters only if they contain complex logic that you want to prove with a test.
9. Do not test for null reference exceptions on called methods. Add a Guard.NotNull instead. 
   These are generally coding mistakes that can be taken out when code reviewing and testing this does not add much value.
10. If a class you're trying to test is abstract create a private class inside the test class that inherits from the abstract class.
   Create methods with the new keyword that calls the method on the abstract class and passes on the parameters. This enables you to test the abstract class.
   ```csharp
   public abstract class BaseRepository
   {
		IDbConnection connection;
		
		protected BaseRepository(IDbConnection connection)
		{
			Guard.NotNull(connection, nameof(connection));
		
			this.connection = connection;
		}
   
		protected IDbContext GetContext()
		{
			return this.connection.Context;
		}
   }
   
   public class BaseRepositoryTest
   {
		[Fact]
		public void GetContextReturnsContext()
		{
			var connection = new DbConnection();
			var dbConnectionMock = new Mock<IDbConnection>();
			dbConnectionMock.Setup(d=> d.Context)
				.Returns(connection);			
			
			var repository = new BaseRepositoryStub(dbConnectionMock.Object);
			var result = repository.GetContext();
			
			Assert.Equal(connection, result);
		}
   
		private class BaseRepositoryStub : BaseRepository
		{
			public BaseRepositoryStub(IDbConnection connection): base(connection)
			{
			}
			
			public new IDbContext GetContext()
			{
				return base.GetContext();
			}
		}
   }
   ```



