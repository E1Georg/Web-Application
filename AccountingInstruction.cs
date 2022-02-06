using System;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecurityDepartment.Models
{
    [Table("AccountingInstructions")]
    public class AccountingInstruction    {     
        
        [Key]
        public int RegNumber { get; set; }

        [DataType(DataType.Date)]        
        [Remote(action: "CheckDate", controller: "AccountingInstruction", ErrorMessage = "Некорректная дата")]
        public DateTime Date { get; set; }
    
        public int ObjectId { get; set; }   
        public int ExecutorId { get; set; }   
        public int ListenerId { get; set; }
        public int InstructionId { get; set; }

        [ForeignKey("InstructionId")]
        public virtual Instruction Instruction { get; set; }

        [ForeignKey("ObjectId")]
        public virtual objectCard ObjectCards { get; set; }

        [ForeignKey("ExecutorId")]
        public virtual workerCard WorkerCards { get; set; }

        [ForeignKey("ListenerId")]
        public virtual Client Clients { get; set; }
    }
}
