using AutoTrade.Core.DTOs;
using AutoTrade.Core.Entities;
using AutoTrade.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace AutoTrade.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IGenericRepository<Role> _roleRepository;

        public RolesController(IGenericRepository<Role> roleRepository)
        {
            _roleRepository = roleRepository;
        }

        // GET: api/Roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
        {
            var roles = await _roleRepository.GetAllAsync();

            var roleDtos = roles.Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description
            });

            return Ok(roleDtos);
        }

        // GET: api/Roles/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<RoleDto>> GetRole(Guid id)
        {
            var role = await _roleRepository.GetByIdAsync(id);

            if (role == null)
                return NotFound($"ID'si {id} olan rol bulunamadı.");

            var roleDto = new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description
            };

            return Ok(roleDto);
        }

        // POST: api/Roles
        [HttpPost]
        public async Task<ActionResult<RoleDto>> CreateRole([FromBody] CreateRoleDto createRoleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var role = new Role
            {
                Id = Guid.NewGuid(),
                Name = createRoleDto.Name,
                Description = createRoleDto.Description
            };

            await _roleRepository.AddAsync(role);

            var roleDto = new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description
            };

            return CreatedAtAction(nameof(GetRole), new { id = role.Id }, roleDto);
        }

        // PUT: api/Roles/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateRoleDto updateRoleDto)
        {
            var role = await _roleRepository.GetByIdAsync(id);

            if (role == null)
                return NotFound($"ID'si {id} olan rol bulunamadı.");

            role.Name = updateRoleDto.Name;
            role.Description = updateRoleDto.Description;

            _roleRepository.Update(role);

            return NoContent();
        }

        // DELETE: api/Roles/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteRole(Guid id)
        {
            var role = await _roleRepository.GetByIdAsync(id);

            if (role == null)
                return NotFound($"ID'si {id} olan rol bulunamadı.");

            _roleRepository.Delete(role);

            return NoContent();
        }
    }
}