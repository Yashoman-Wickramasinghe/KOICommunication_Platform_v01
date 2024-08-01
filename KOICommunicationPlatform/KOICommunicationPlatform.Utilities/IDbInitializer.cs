using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.Utilities
{
    //-- 12203888 Edited by W A Yashoman Wickramasinghe -- //
    //Purpose of this interface : Create a defauld user admin
    //Holds number of users for the website : Initialize();
    //public void Initialize(); : Does not required public because of an interface
    public interface IDbInitializer
    {
        void Initialize();
    }
}
