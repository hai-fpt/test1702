using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using test1702.Models;

namespace test1702.Controllers
{
    public class ContactsController : Controller
    {
        private string apiUrl = "http://localhost:1234/api/contacts";
        private HttpClient client = new HttpClient();
        
        public async Task<ActionResult> Index()
        {
            var response = await client.GetAsync(apiUrl);
            var contacts = await response.Content.ReadAsAsync<List<Contact>>();

            contacts = contacts.OrderBy(c => c.Name).ToList();

            return View(contacts);
        }
        private async Task<List<Group>> GetGroups()
        {
            var groupsUrl = "http://localhost:1234/api/groups";
            var response = await client.GetAsync(groupsUrl);
            return await response.Content.ReadAsAsync<List<Group>>();
        }
        public async Task<ActionResult> Create()
        {
            var group = await GetGroups();
            ViewBag.Groups = new SelectList(group, "Name", "Name");
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create([Bind(Include = "Name, Number, GroupName")] Contact contact)
        {
            var group = await GetGroups();
            ViewBag.Groups = new SelectList(group, "Name", "Name");

            if (ModelState.IsValid)
            {
                var response = await client.PostAsJsonAsync(apiUrl, contact);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    ModelState.AddModelError("", await response.Content.ReadAsStringAsync());
                }
                else
                {
                    ModelState.AddModelError("", "An error has occured");
                }
            }

            return View(contact);
        }

        public async Task<ActionResult> Edit (int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            var response = await client.GetAsync($"{apiUrl}/{id}");
            if (response.IsSuccessStatusCode)
            {
                var contact = await response.Content.ReadAsAsync<Contact>();
                var groups = await GetGroups();
                ViewBag.Groups = new SelectList(groups, "Name", "Name", contact.GroupName);

                return View(contact);
            }
            else
            {
                return HttpNotFound();
            }
        }
        [HttpPost]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Number,GroupName")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                var response = await client.PutAsJsonAsync($"{apiUrl}/{contact.Id}", contact);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    ModelState.AddModelError("", await response.Content.ReadAsStringAsync());
                }
                else
                {
                    ModelState.AddModelError("", "An error occurred while updating the contact");
                }
            }

            var groups = await GetGroups();
            ViewBag.Groups = new SelectList(groups, "Name", "Name", contact.GroupName);

            return View(contact);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var response = await client.GetAsync($"{apiUrl}/{id}");

            if (response.IsSuccessStatusCode)
            {
                var contact = await response.Content.ReadAsAsync<Contact>();
                return View(contact);
            }
            else
            {
                return HttpNotFound();
            }
        }

        [HttpPost, ActionName("Delete")]
        
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var response = await client.DeleteAsync($"{apiUrl}/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return HttpNotFound();
            }
        }


    }
}