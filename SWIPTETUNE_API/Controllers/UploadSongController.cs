using BusinessObject;
using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace SWIPTETUNE_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadSongController : ControllerBase
    {
        private readonly SWIPETUNEDbContext _context;
        public UploadSongController(SWIPETUNEDbContext context)
        {
            _context = context;
        }

        [HttpPost("convertToByte")]
        public async Task<IActionResult> ConvertToByte(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest("No files uploaded.");
            }

            var fileDataList = new List<SongFile>();

            foreach (var file in files)
            {
                if (file.Length == 0)
                {
                    continue;
                }

                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    byte[] fileBytes = memoryStream.ToArray();

                    string fileName = file.FileName.Split("-")[0];

                    var fileData = new SongFile
                    {
                        FileName = fileName,
                        FileData = fileBytes
                    };

                    fileDataList.Add(fileData);
                }
            }
            foreach (var songs in fileDataList)
            {
                if (_context.SongsFile.SingleOrDefault(x=>x.FileName == songs.FileName)==null)
                {
                    await _context.SongsFile.AddAsync(songs);
                    await _context.SaveChangesAsync();
                }else
                {
                    return BadRequest($"Exist that song {songs.FileName}");
                }
            }

            return Ok(new
            { file = fileDataList.Select(x => x.FileName).ToList() });
        }
        [HttpGet]
        [Route("GetSongFiles")]
        public async Task<List<SongFile>> GetSongFiles()
        {
            var list = new List<SongFile>();
            try
            {
                list= await _context.SongsFile.ToListAsync();
            }catch(Exception ex) 
            {
                throw new Exception(ex.Message);
            }
            return list;
        }
    }
}

