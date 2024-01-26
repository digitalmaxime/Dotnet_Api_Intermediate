using Singleton;

Console.Title = "Singleton";

Logger.Instance.Log("Hello");
var instance1 = Logger.Instance;
var instance2 = Logger.Instance;

instance1.Log(instance1.GetHashCode().ToString());
instance2.Log(instance1.GetHashCode().ToString());