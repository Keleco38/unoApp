using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DomainObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HallOfFameController : ControllerBase
    {
        private readonly IHallOfFameRepository _hallOfFameRepository;
        private readonly IMapper _mapper;

        public HallOfFameController(IHallOfFameRepository hallOfFameRepository, IMapper mapper)
        {
            _hallOfFameRepository = hallOfFameRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var topFifty = _hallOfFameRepository.GetTopFiftyPlayers();
            var topFiftyDto = _mapper.Map<List<HallOfFameDto>>(topFifty);
            return Ok(topFiftyDto);
        }
    }
}