using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace backend2.Exceptions
{
  [Serializable]
  public class DeploymentNotFoundException : Exception
  {
    //
    // For guidelines regarding the creation of new exception types, see
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
    // and
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
    //

    public DeploymentNotFoundException()
    {
    }

    public DeploymentNotFoundException(string message) : base(message)
    {
    }

    public DeploymentNotFoundException(string message, Exception inner) : base(message, inner)
    {
    }

    protected DeploymentNotFoundException(
      SerializationInfo info,
      StreamingContext context) : base(info, context)
    {
    }
  }
}