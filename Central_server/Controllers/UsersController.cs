using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Central_server.Data;
using Central_server.ViewModels;
using E_Commerce.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Central_server.Controllers
{
    public class UsersController : Controller
    {
        private readonly FarmTrackingContext _context;

        public UsersController(FarmTrackingContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var t = _context.Users.ToListAsync();
            await t;
            var result = t.Result.Select(x => new UserVM
            {
                UserName = x.UserName,
                
                UserId = x.UserId,
                FullName = x.FullName,
                StudentId = x.StudentId,
                PhotoUrl = x.PhotoUrl
            });

            return View(result);
        }

        // GET: Users/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }
            var result = new UserVM
            {
                UserId = user.UserId,
                FullName = user.FullName,
                StudentId = user.StudentId,
                PhotoUrl = user.PhotoUrl
            };


            return View(result);
        }

        // GET: Users/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserVM item)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    FullName = item.FullName,
                    StudentId = item.StudentId,
                };
                user.UserName = item.UserName;
                user.Salt = System.Text.Encoding.UTF8.GetBytes(MyUtil.GenarateRandomKey());
                user.PasswordHash = System.Text.Encoding.UTF8.GetBytes(DataEncryptionExtensions.ToMd5Hash(item.Password, Convert.ToBase64String(user.Salt)));
                if (item.Photo != null)
                {
                    var directoryPath = Path.Combine("wwwroot/images/users");
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    var filePath = Path.Combine(directoryPath, item.Photo.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await item.Photo.CopyToAsync(stream);
                    }
                    item.PhotoUrl = item.Photo.FileName;

                    user.PhotoUrl = item.PhotoUrl;
                }

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        // GET: Users/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var result = new EditVM
            {
                UserId = user.UserId,
                FullName = user.FullName,
                StudentId = user.StudentId,
                PhotoUrl = user.PhotoUrl,
                UserName = user.UserName
            };

            return View(result);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditVM item)
        {
            if (id != item.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _context.Users.FirstOrDefaultAsync(p => p.UserId == id);
                    if (user == null)
                    {
                        return NotFound();
                    }
                    user.UserName = item.UserName;
                    user.FullName = item.FullName;
                    user.StudentId = item.StudentId;
                    user.PhotoUrl = item.PhotoUrl;
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(item.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        // GET: Users/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }
            var result = new UserVM
            {
                UserId = user.UserId,
                FullName = user.FullName,
                StudentId = user.StudentId,
                PhotoUrl = user.PhotoUrl
            };

            return View(result);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        public IActionResult Login(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            LoginVM item = new LoginVM();
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM item, string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(p => p.UserName == item.UserName);
                if (user == null)
                {
                    ModelState.AddModelError("UserName", "Tên đăng nhập không tồn tại");
                    return View(item);
                }
                var passwordHash = System.Text.Encoding.UTF8.GetBytes(DataEncryptionExtensions.ToMd5Hash(item.Password, Convert.ToBase64String(user.Salt)));
                if (!passwordHash.SequenceEqual(user.PasswordHash))
                {
                    ModelState.AddModelError("Password", "Mật khẩu không đúng");
                    return View(item);
                }
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Role, "Thành viên nhóm"),
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(claimsPrincipal);
                if (string.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToAction("Index");
                }
                return Redirect(returnUrl);
                
            }
            return View(item);
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == int.Parse(userId));
            if (user == null)
            {
                return NotFound();
            }

            var result = new UserVM
            {
                UserId = user.UserId,
                FullName = user.FullName,
                StudentId = user.StudentId,
                PhotoUrl = user.PhotoUrl,
                UserName = user.UserName,
            };

            return View(result);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
