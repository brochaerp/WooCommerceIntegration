using AcendaSDK.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AcendaSDK.Service
{
   public interface IService
    {
        T GetAll<T>() where T :  new ();
        T GetById<T>(string id) where T : new ();
        BaseDTO Create(object data);
        BaseDTO Update(string id, object data);
        BaseDTO Delete(string id);

        
    }
}
