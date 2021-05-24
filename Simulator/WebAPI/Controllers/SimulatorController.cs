using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    // acest Web API are o functionalitate foarte simpla, primeste date de la aplicatia monitor, le stocheaza in variabila statica stamp
    // si poate sa furnizeze aceste date unui consumer (aplicatie mobila, aplicatie web, aplicatie desktop).
    // comunicarea se efectueaza apelang metodele din controller sub formalocalhost...../api/simulator
         
    [Route("api/[controller]")]
    [ApiController]
    public class SimulatorController : ControllerBase
    {
        private static List<Stamp> stamps = null;

        public SimulatorController()
        {
            if(stamps==null)
            {
                stamps = new List<Stamp>();
            }
        }
        
        //private static object ReceivedData;
        // GET: api/Simulator
        [HttpGet]
        public IEnumerable<Stamp> Get()
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            //return new string[] { "value1", "value2" };
            return stamps;
        }

        // GET: api/Simulator/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Simulator    
        [HttpPost()]
        public void Post([FromBody] Stamp value)
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            stamps.Add(value);
        }

        // PUT: api/Simulator/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }    
}
