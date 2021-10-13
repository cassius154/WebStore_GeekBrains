using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebStore.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly Dictionary<int, string> _values = Enumerable.Range(1, 10)  //генерим 10 значений от 1 до 10
            .Select(i => (Id: i, Value: $"Value-{i}"))
            .ToDictionary(v => v.Id, v => v.Value);

        //разные варианты

        //[HttpGet]
        //public IEnumerable<string> Get() => _values.Values;

        //[HttpGet]
        //public ActionResult<string[]> Get() => _values.Values.ToArray();

        //[HttpGet]
        //public ActionResult<string[]> Get() => Ok(_values.Values.ToArray());

        [HttpGet]
        public IActionResult Get() => Ok(_values.Values);

        [HttpGet("{id}")]  //это передается в адресной строке
        public IActionResult GetById(int id)
        {
            if (!_values.ContainsKey(id))
            {
                return NotFound();
            }

            return Ok(_values[id]);
        }

        [HttpGet("count")] //адрес для вызова этого метода
        public IActionResult Count() => Ok(_values.Count);

        [HttpPost]
        [HttpPost("add")]  //еще один адрес для вызова этого метода
        public IActionResult Add([FromBody] string value)
        {
            var id = _values.Count == 0 ? 1 : _values.Keys.Max() + 1;
            _values[id] = value;

            //CreatedAtAction = код 201, + указание адреса, куда надо перейти, чтобы получить созданное
            //return CreatedAtAction(nameof(GetById), new { id }, _values[id]); 
            //var o = (Id: id, Value: _values[id]);

            //непонятно - возвращается созданный тут объект, а не результат GetById(id)
            //для чего тут тогда GetById? (адрес GetById возвращается в Location заголовка ответа)
            var o = new { Id = id, Value = _values[id] };
            return CreatedAtAction(nameof(GetById), new { id }, o);
        }

        [HttpPost("addfromquery")]  //адрес для вызова этого метода
        public IActionResult AddFromQuery(string value)
        {
            var id = _values.Count == 0 ? 1 : _values.Keys.Max() + 1;
            _values[id] = value;

            //CreatedAtAction = код 201, + указание адреса, куда надо перейти, чтобы получить созданное
            //return CreatedAtAction(nameof(GetById), new { id }, _values[id]); 
            //var o = (Id: id, Value: _values[id]);

            //непонятно - возвращается созданный тут объект, а не результат GetById(id)
            //для чего тут тогда GetById? (адрес GetById возвращается в Location заголовка ответа)
            var o = new { Id = id, Value = _values[id] };
            return CreatedAtAction(nameof(GetById), new { id }, o);
        }

        [HttpPut("{id}")]  //это передается в адресной строке
        public IActionResult Replace(int id, [FromBody] string value)  //остальные параметры будут переданы как параметры запроса или тело (форма)
        {
            if (!_values.ContainsKey(id))
            {
                return NotFound();
            }

            _values[id] = value;

            return Ok(new { Id = id, Value = _values[id] });
        }

        [HttpPut("replacefromquery/{id}")] //еще один адрес для вызова этого метода (id передается в адресной строке)
        public IActionResult ReplaceFromQuery(int id, string value)  //остальные параметры будут переданы как параметры запроса или тело (форма)
        {
            if (!_values.ContainsKey(id))
            {
                return NotFound();
            }

            _values[id] = value;

            return Ok(new { Id = id, Value = _values[id] });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!_values.ContainsKey(id))
            {
                return NotFound();
            }

            _values.Remove(id);

            return Ok();
        }
    }
}
