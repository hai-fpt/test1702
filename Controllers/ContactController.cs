using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using test1702.Models;

namespace test1702.Controllers
{
    [RoutePrefix("api/contacts")]
    public class ContactController : ApiController
    {
        private ContactContext db = new ContactContext();
        private readonly HttpClient client;
        private readonly string apiUrl;

        public ContactController()
        {
            client = new HttpClient();
            apiUrl = "http://localhost:1234/api/contacts";
        }
        [HttpGet]
        public IHttpActionResult GetContacts()
        {
            var contacts = db.Contacts.ToList();
            return Ok(contacts);
        }

        [HttpPost]
        public IHttpActionResult PostContacts(Contact contact)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (db.Contacts.Any(c => c.Name == contact.Name))
            {
                return BadRequest("No duplicate names");
            }

            db.Contacts.Add(contact);
            db.SaveChanges();
            
            return CreatedAtRoute("DefaultApi", new {id = contact.Id}, contact);
        }

        [HttpPut]
        public IHttpActionResult PutContacts(int id, Contact contact)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != contact.Id)
            {
                return BadRequest("Contact Id doesn't exists");
            }

            var existingContact = db.Contacts.Find(id);
            if (existingContact == null)
            {
                return BadRequest("Not found");
            }

            if (db.Contacts.Any(c => c.Name == contact.Name && c.Id != contact.Id))
            {
                return BadRequest("Contact name already exists");
            }

            existingContact.Name = contact.Name;
            existingContact.Number = contact.Number;
            existingContact.GroupName = contact.GroupName;

            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpDelete]
        public IHttpActionResult DeleteContact(int id)
        {
            var contact = db.Contacts.Find(id);
            if (contact == null)
            {
                return NotFound();
            }
            db.Contacts.Remove(contact);
            db.SaveChanges();

            return Ok(contact);
        }
    }

}
