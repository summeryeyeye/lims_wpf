using AutoMapper;
using Lims.WebAPI.Service;
using Lims.WebAPI.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Lims.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MyBaseController<T, TDto> : ControllerBase where T : class, new()
    {
        private readonly IMapper mapper;
        private readonly IBaseService<T> baseService;
        public MyBaseController(IMapper mapper, BaseService<T> baseService)
        {
            this.mapper = mapper;
            this.baseService = baseService;
        }
        
        [HttpGet]
        public ActionResult GetMethods()
        {         
            Type t =this.GetType();//StatController是指定控制器的名称
            var ControllerMethods = t.GetMethods().Where(m=>m.Module.Name=="Lims.WebAPI.dll");
            List<string> strings = new List<string>();
            foreach (var item in ControllerMethods)            
                strings.Add(item.Name);
            return Ok(strings);
        }

        #region BaseAction    
        [HttpGet]
        public async Task<ApiResponse> GetAll() => await baseService.QueryAsync();
        // GET api/<ItemsController>/5
        [HttpGet("{id}")]
        public virtual async Task<ApiResponse> GetSingle(string id) => await baseService.SearchAsync(id);
        // DELETE api/<ItemsController>/5
        [HttpDelete("{id}")]
        public async Task<ApiResponse> Delete(string id) => await baseService.DeleteAsync(id);
        // PUT api/<ItemsController>/5
        [HttpPost]
        public async Task<ApiResponse> Update([FromBody] TDto param) => await baseService.EditAsync(mapper.Map<T>(param));

        [HttpPost]
        public async Task<ApiResponse> UpdateRange([FromBody] List<TDto> param) => await baseService.EditRangeAsync(mapper.Map<List<T>>(param));
        // POST api/<ItemsController>
        [HttpPost]
        public async Task<ApiResponse> Create([FromBody] TDto itemDto) => await baseService.CreateAsync(mapper.Map<T>(itemDto));
        #endregion
    }
}
