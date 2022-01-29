using ControlWork10.Entyties;
using ControlWork10.Models;
using ControlWork10.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace ControlWork10.Controllers
{
    public class PlaceController : Controller
    {
        private ApplicationDbContext _context;
        public PlaceController(IServiceProvider serviceProvider)
        {
            _context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        }
        public IActionResult Index()
        {
            var places = _context.Places.ToList();
            foreach (var item in places)
                item.AverageRating = Math.Round(item.AverageRating, 1);
                return View(places);
        }
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]

        public  IActionResult Add(Place place)
        {
            if (place != null)
            {
                place.UserId = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name).Id;
                place.ReviewsCount = 0;
                place.AverageRating = 0;
                place.PhotoCount = 0;
                _context.Add(place);
                _context.SaveChanges();
            }
            var photoGallery = new PhotoGallery()
            {
                PlaceId = place.Id,
            };
            _context.Add(photoGallery);
            _context.SaveChanges();
            place.PhotoGalleryId = photoGallery.Id;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public  IActionResult Details(int id)
        {
            var details = new DetailsViewModel()
            {
                Place = _context.Places.FirstOrDefault(p => p.Id == id),
                Reviews = _context.Reviews.Where(r=>r.PlaceId == id).OrderByDescending(r=>r.DateTime).ToList()
            };
            details.Place.AverageRating = Math.Round(details.Place.AverageRating, 1);

            if (details.Place.PhotoCount != 0)
                details.Photos = _context.Photos.Where(p => p.GalleryId == details.Place.PhotoGalleryId).ToList();
            details.Place.User = _context.Users.FirstOrDefault(u=>u.Id == details.Place.UserId);
            return View(details);
        }
        public IActionResult AddReview(int placeId,int rating,string text)
        {
            if (_context.Reviews.Where(r => r.PlaceId == placeId && r.UserName == User.Identity.Name).ToList().Count != 0)
                return Ok("-1");
            else
            {
                var review = new Review()
                {
                    UserName = User.Identity.Name,
                    PlaceId = placeId,
                    Rating = rating,
                    TextReview = text,
                    DateTime = DateTime.Now
                };
                _context.Places.FirstOrDefault(p => p.Id == placeId).ReviewsCount += 1;
                _context.Reviews.Add(review);
                _context.SaveChanges();
                _context.Places.FirstOrDefault(p => p.Id == placeId).AverageRating = SetAverageRating(placeId);
                
                
                _context.SaveChanges();
                return Ok("200");
            }
            
        }
        public IActionResult DeleteReview(int placeId)
        {
            _context.Remove(_context.Reviews.FirstOrDefault(r => r.PlaceId == placeId && r.UserName == User.Identity.Name));
            _context.Places.FirstOrDefault(p => p.Id == placeId).ReviewsCount -= 1;
            _context.SaveChanges();
            _context.Places.FirstOrDefault(p => p.Id == placeId).AverageRating = SetAverageRating(placeId);
            _context.SaveChanges();
                return Ok("200");

        }
        public IActionResult AddPhoto(int placeId, string link)
        {
            var photo = new Photo()
            {
                GalleryId = _context.Places.FirstOrDefault(p => p.Id == placeId).PhotoGalleryId,
                Link = link
            };
            _context.Photos.Add(photo);
            _context.Places.FirstOrDefault(p => p.Id == placeId).PhotoCount += 1;
            _context.SaveChanges();
            return Ok("200");

        }
        public double SetAverageRating(int placeId)
        {
            double rating = 0;
            var reviews = _context.Reviews.Where(r => r.PlaceId == placeId).ToList();
            foreach (var item in reviews)
            {
                rating += item.Rating;
            }
            rating /= reviews.Count;
            return rating;
        }
        public IActionResult Search(string query)
        {
            var resultPlaces = _context.Places
                 .Where(t =>
                     t.Description.Contains(query) ||
                     t.Name.Contains(query)).ToList();
            return View(resultPlaces);
        }
    }
}
