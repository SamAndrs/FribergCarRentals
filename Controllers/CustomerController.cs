using FribergRentalCars.Data;
using FribergRentalCars.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FribergRentalCars.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomer _repository;

        public CustomerController(ICustomer customerRepository)
        {
            this._repository = customerRepository;
        }


        // GET: CustomerController
        public async Task<IActionResult> Index()
        {
            var customers = await _repository.GetAllAsync();
            return View(customers);
        }

        // GET: CustomerController/Details/5
        //public async Task <IActionResult> Details(int id)
        public async Task <IActionResult> Details(int id)
        {
            /*var customer = await _repository.GetIdByAsync(id);*/
            var customer = await _repository.GetWithAdressAsync(id);
            if(customer == null)
            {
                NotFound();
            }
            return View(customer);
        }

        // GET: CustomerController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CustomerController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        /*public ActionResult Create([Bind("FirstName, LastName, PhoneNumber, Email, Bookings," +
            "Adress.Street, Adress.City, Adress.PostalCode")] Customer customer)*/
        public async Task<ActionResult> Create(Customer customer)
          {
            if (ModelState.IsValid)
            {
                //return RedirectToAction(nameof(Index));
                try
                {
                    await _repository.AddAsync(customer);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error creating customer: {ex.Message}");
                }
            }
            return View(customer);
        }

        // GET: CustomerController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if(id == null)
            {
                return NotFound();
            }
            /*var customer = await _repository.GetIdByAsync(id);*/
            var customer = await _repository.GetWithAdressAsync(id);

            if(customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: CustomerController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Customer customer)
        {
            if(id != customer.CustomerId)
            {
                NotFound();
            }

            if(ModelState.IsValid)
            {
                try
                {
                    await _repository.UpdateAsync(customer);
                }
                catch(DbUpdateConcurrencyException)
                {
                    if(customer.CustomerId == null)
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
            return View(customer);

            
        }

        // GET: CustomerController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CustomerController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Customer customer)
        {
            try
            {
                _repository.DeleteAsync(customer);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
