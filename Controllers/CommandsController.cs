using System.ComponentModel.Design;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepo _repository;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId){
            Console.WriteLine($"--> Fetching commands for platform ID: {platformId}");
            if(!_repository.PlatformExists(platformId)){
                return NotFound();
            }
            var commands = _repository.GetCommandsForPlatform(platformId);
            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
        }
        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId){
            Console.WriteLine($"--> Fetching command {commandId} for platformId {platformId}");
            if(!_repository.PlatformExists(platformId)){
                return NotFound();
            }
            var command = _repository.GetCommand(platformId, commandId);
            if(command == null){
                return NotFound();
            }
            return Ok(_mapper.Map<CommandReadDto>(command));
        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto command){
            Console.WriteLine($"--> Creating command for platformId {platformId}");
            if(!_repository.PlatformExists(platformId)){
                return NotFound();
            }
            var comm = _mapper.Map<Command>(command);
            _repository.CreateCommand(platformId, comm);
            _repository.SaveChanges();

            var commandReadDto = _mapper.Map<CommandReadDto>(comm);
            return CreatedAtRoute(nameof(GetCommandForPlatform),
                new {platformId = platformId, CommandID = commandReadDto.Id}, commandReadDto);
        }
    }
}