using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MyBusinessNamespace {


  /// <summary>
  /// this is a summary with <see href="https://github.com/KornSW/MDgen">Hyperlink</see>
  /// </summary>
  public interface IFooAPI : System.Data.Fuse.IRepository<TestModel, string> {

    bool Foooo(string a, out int b);

    TestModel Kkkkkk(int optParamA = 0, string optParamB = "f" );

    /// <summary>
    /// Meth
    /// </summary>
    /// <param name="errorCode"> Bbbbbb </param>
    void AVoid(TestModel errorCode);

    bool TestNullableDt(DateTime? dt);

  }

  public interface IApiBase{

    /// <summary>
    /// descibes if the base is usable
    /// </summary>
    /// <returns></returns>
    bool BaseIsUsable();

  }

  /// <summary>
  /// MMMMMMMMMMMMMMMMMMM
  /// </summary>
  public class TestModel {

    /// <summary>
    /// jfjfj
    /// </summary>
    [Required()]
    public String FooBar { get; set; } = "default";

    [Required()]
    public TestNode RootNode { get; set; } = null;


    [Required()]
    public String[] fff { get; set; } = null;

  }

  /// <summary>
  /// fsdfsf
  /// </summary>
  public class TestNode {

    [Required()]
    public TestNode ChildNode { get; set; } = null;



  }





}
