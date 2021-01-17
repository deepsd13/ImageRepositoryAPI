using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImageRepository.Model;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace ImageRepository.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly ImageDbContext _context;
        private readonly IWebHostEnvironment _webHostEnviornment;

        public ImageController(ImageDbContext context, IWebHostEnvironment webHostEnviornment)
        {
            _context = context;
            this._webHostEnviornment = webHostEnviornment;
        }

        // GET: api/Image
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ImageModel>>> GetImages()
        {
            return await _context.Images
                .Select(x=> new ImageModel() {
                    Id = x.Id,
                    ImageTitle = x.ImageTitle,
                    ImageName = x.ImageName,
                    ImageSrc = String.Format("{0}://{1}{2}/Images/{3}",Request.Scheme, Request.Host, Request.PathBase,x.ImageName)
                })
                .ToListAsync();
        }

        // GET: api/Image/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ImageModel>> GetImageModel(int id)
        {
            var imageModel = await _context.Images.FindAsync(id);

            if (imageModel == null)
            {
                return NotFound();
            }

            return imageModel;
        }

        // PUT: api/Image/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutImageModel(int Id, ImageModel imageModel)
        {
            if (Id != imageModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(imageModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ImageModelExists(Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Image
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<ImageModel>> PostImageModel([FromForm]ImageModel imageModel)
        {

            //passing the file that was sent through the API to the async method 
            imageModel.ImageName = await SaveImage(imageModel.ImageFile);
            _context.Images.Add(imageModel);
            await _context.SaveChangesAsync();

            return StatusCode(201);
        }

        // DELETE: api/Image/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ImageModel>> DeleteImageModel(int id)
        {
            var imageModel = await _context.Images.FindAsync(id);
            if (imageModel == null)
            {
                return NotFound();
            }

            DeleteImage(imageModel.ImageName);
            _context.Images.Remove(imageModel);
            await _context.SaveChangesAsync();

            return imageModel;
        }

        private bool ImageModelExists(int Id)
        {
            return _context.Images.Any(e => e.Id == Id);
        }

        
        //method which will upload the image to the project which then can be used for uploading to database. 
        [NonAction]
        public async Task<string> SaveImage(IFormFile imageFile) {

            string imageName = new String(Path.GetFileNameWithoutExtension(imageFile.FileName).Take(10).ToArray()).Replace(' ', '-');
            //making sure every image name is unique
            imageName = imageName + DateTime.Now.ToString("yymmssff") + Path.GetExtension(imageFile.FileName); 
            var imagePath = Path.Combine(_webHostEnviornment.ContentRootPath, "Images", imageName);
            using (var fileStream = new FileStream(imagePath, FileMode.Create)) {
                await imageFile.CopyToAsync(fileStream);
            }
            return imageName;
        }

        [NonAction]
        public void DeleteImage (string imageName) {
            var imagePath = Path.Combine(_webHostEnviornment.ContentRootPath, "Images", imageName);
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);
        }
    }
}
