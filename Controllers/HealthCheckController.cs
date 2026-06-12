using Microsoft.AspNetCore.Mvc;

namespace DataIntegrationHub.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class HealthCheckController : ControllerBase
    {
        private readonly ILogger<HealthCheckController> _logger;

        // Inyeccion de dependencias (DI)
        public HealthCheckController(ILogger<HealthCheckController> logger) => _logger = logger;

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("Health check endpoint was called.");
            return Ok(new {Status = "Healthy", Timestamp = DateTime.Now});
        }
    }
}
