using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SecurityDepartment.Data;
using SecurityDepartment.Models;

namespace SecurityDepartment.Controllers
{
    public class AccountingInstructionsController : Controller
    {
        private readonly MyDBContext _context;

        public AccountingInstructionsController(MyDBContext context)
        {
            _context = context;
        }

        // GET: AccountingInstructions
        public async Task<IActionResult> Index(string parametr, string searchString, string sortOrder)
        {
            var myDBContext = from s in _context.AccountingInstruction.Include(a => a.Instruction).Include(a => a.WorkerCards).Include(a => a.ObjectCards).Include(a => a.Clients) select s;
            ViewData["CurrentFilter"] = searchString;

            // Get related tables
            if ((parametr != null) && (parametr == "AddTable"))
            {
                parametr = "Not";
                ViewBag.InstructionModel = new List<Instruction>(_context.Instruction.ToList());                
            }            
            else
            {
                ViewBag.InstructionModel = null;
            }

            // Search by user
            if (!String.IsNullOrEmpty(searchString))
            {
                myDBContext = myDBContext.Where(s => s.Date.ToString().Contains(searchString)
                                       || s.Instruction.Name.Contains(searchString)
                                       || s.ObjectId.ToString().Contains(searchString)
                                       || s.WorkerCards.FirstName.Contains(searchString)
                                       || s.WorkerCards.SecondName.Contains(searchString)
                                       || s.Clients.FirstName.Contains(searchString)
                                       || s.Clients.LastName.Contains(searchString)
                                      );
                return View(await myDBContext.AsNoTracking().ToListAsync());
            }

            // Sorting by user
            ViewData["DateKey"] = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";            
            ViewData["ObjectKey"] = sortOrder == "obj" ? "obj_desc" : "obj";
            ViewData["ExecutorKey"] = sortOrder == "Exec" ? "exec_desc" : "Exec";
            ViewData["ListenerKey"] = sortOrder == "Listener" ? "Listener_desc" : "Listener";
            ViewData["LessKey"] = sortOrder == "Less" ? "Less_desc" : "Less"; 

            switch (sortOrder)
            {
                case "date_desc":
                    myDBContext = _context.AccountingInstruction.Include(a => a.Instruction).Include(a => a.WorkerCards).Include(a => a.ObjectCards).Include(a => a.Clients).OrderByDescending(s => s.Date);
                    break;
                case "obj":
                    myDBContext = _context.AccountingInstruction.Include(a => a.Instruction).Include(a => a.WorkerCards).Include(a => a.ObjectCards).Include(a => a.Clients).OrderBy(s => s.ObjectId);
                    break;
                case "obj_desc":
                    myDBContext = _context.AccountingInstruction.Include(a => a.Instruction).Include(a => a.WorkerCards).Include(a => a.ObjectCards).Include(a => a.Clients).OrderByDescending(s => s.ObjectId);
                    break;
                case "Exec":
                    myDBContext = _context.AccountingInstruction.Include(a => a.Instruction).Include(a => a.WorkerCards).Include(a => a.ObjectCards).Include(a => a.Clients).OrderBy(s => s.WorkerCards.SecondName);
                    break;
                case "exec_desc":
                    myDBContext = _context.AccountingInstruction.Include(a => a.Instruction).Include(a => a.WorkerCards).Include(a => a.ObjectCards).Include(a => a.Clients).OrderByDescending(s => s.WorkerCards.SecondName);
                    break;
                case "Listener":
                    myDBContext = _context.AccountingInstruction.Include(a => a.Instruction).Include(a => a.WorkerCards).Include(a => a.ObjectCards).Include(a => a.Clients).OrderBy(s => s.Clients.LastName);
                    break;
                case "Listener_desc":
                    myDBContext = _context.AccountingInstruction.Include(a => a.Instruction).Include(a => a.WorkerCards).Include(a => a.ObjectCards).Include(a => a.Clients).OrderByDescending(s => s.Clients.LastName);
                    break;
                case "Less":
                    myDBContext = _context.AccountingInstruction.Include(a => a.Instruction).Include(a => a.WorkerCards).Include(a => a.ObjectCards).Include(a => a.Clients).OrderBy(s => s.Instruction.Name);
                    break;
                case "Less_desc":
                    myDBContext = _context.AccountingInstruction.Include(a => a.Instruction).Include(a => a.WorkerCards).Include(a => a.ObjectCards).Include(a => a.Clients).OrderByDescending(s => s.Instruction.Name);
                    break;

                default:
                    myDBContext = _context.AccountingInstruction.Include(a => a.Instruction).Include(a => a.WorkerCards).Include(a => a.ObjectCards).Include(a => a.Clients).OrderBy(s => s.Date);
                    break;
            }

            return View(await myDBContext.ToListAsync());
        }

        // GET: AccountingInstructions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountingInstruction = await _context.AccountingInstruction.Include(a => a.Instruction).Include(a => a.WorkerCards).Include(a => a.ObjectCards).Include(a => a.Clients).FirstOrDefaultAsync(m => m.RegNumber == id);
            if (accountingInstruction == null)
            {
                return NotFound();
            }

            return View(accountingInstruction);
        }

        // GET: AccountingInstructions/Create
        public IActionResult Create()
        {
            ViewData["ObjectId"] = new SelectList(_context.objectCards, "Id", "Id");
            ViewData["ExecutorId"] = new SelectList(_context.workerCards, "Id", "Id");
            ViewData["ListenerId"] = new SelectList(_context.Clients, "Id", "Id");
            ViewData["InstructionId"] = new SelectList(_context.Instruction, "Id", "Name");
            return View();
        } 

      // POST: AccountingInstructions/Create
      [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RegNumber,Date,ObjectId,ExecutorId,ListenerId,InstructionId")] AccountingInstruction accountingInstruction)
        {
            if (ModelState.IsValid)
            {
                _context.Add(accountingInstruction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["InstructionId"] = new SelectList(_context.Instruction, "Id", "Id", accountingInstruction.InstructionId);
            return View(accountingInstruction);
        }

        // GET: AccountingInstructions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountingInstruction = await _context.AccountingInstruction.FindAsync(id);
            if (accountingInstruction == null)
            {
                return NotFound();
            }
            ViewData["InstructionId"] = new SelectList(_context.Instruction, "Id", "Id", accountingInstruction.InstructionId);
          
            return View(accountingInstruction);
        }

        // POST: AccountingInstructions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RegNumber,Date,ObjectId,ExecutorId,ListenerId,InstructionId")] AccountingInstruction accountingInstruction)
        {
            if (id != accountingInstruction.RegNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(accountingInstruction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountingInstructionExists(accountingInstruction.RegNumber))
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
            ViewData["InstructionId"] = new SelectList(_context.Instruction, "Id", "Id", accountingInstruction.InstructionId);
            return View(accountingInstruction);
        }

        // GET: AccountingInstructions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountingInstruction = await _context.AccountingInstruction.Include(a => a.Instruction).Include(a => a.WorkerCards).Include(a => a.ObjectCards).Include(a => a.Clients).FirstOrDefaultAsync(m => m.RegNumber == id);
            if (accountingInstruction == null)
            {
                return NotFound();
            }

            return View(accountingInstruction);
        }

        // POST: AccountingInstructions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var accountingInstruction = await _context.AccountingInstruction.FindAsync(id);
            _context.AccountingInstruction.Remove(accountingInstruction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountingInstructionExists(int id)
        {
            return _context.AccountingInstruction.Any(e => e.RegNumber == id);
        }

        public IActionResult Report()
        {
            ViewBag.ReportModel = new List<AccountingInstruction>(_context.AccountingInstruction.Include(a => a.Instruction).ToList());
            return View();
        }

        [AcceptVerbs("Get", "Post")]
        public IActionResult CheckDate(DateTime Date)
        {
            DateTime currentDateMin = DateTime.Now;
            DateTime currentDateMax = currentDateMin.AddDays(2);
            currentDateMin = currentDateMin.AddYears(-5);

            if ((Date < currentDateMin) || (Date > currentDateMax))
            {
                return Json(false);
            }

            return Json(true);
        }
    }
}
