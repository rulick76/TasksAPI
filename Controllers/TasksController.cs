using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TasksAPI.Models;

namespace TasksAPI.Controllers
{
    [ApiController]
    [Route("Tasks")]
    public class TaskController : ControllerBase
    {

        private static Object objLock;
        public static Dictionary<string,List<UserTask>> taskHash = new Dictionary<string, List<UserTask>>();
        private readonly ILogger<TaskController> _logger;

        public TaskController(ILogger<TaskController> logger)
        {
            objLock=new Object();
            _logger = logger;
        }

        [HttpGet]
        public Dictionary<string,List<UserTask>> GetAll()
        {
            //Get from Mongo
             
            return  taskHash;
            
        }

        [HttpPost]
        public void Post(UserTask task)
        {
            bool bAdded=false;
           if (!taskHash.ContainsKey(task.UserName)) //Probably a NEW task!
           {
                //user pass inProgress= true for new tasks
                task.Strted=DateTime.Now.ToString();
                List<UserTask> usrLst=new List<UserTask>();
                usrLst.Add(task);
                lock (taskHash)
                {
                    taskHash.Add(task.UserName,usrLst);
                    bAdded=true;
                }
           }
           else //Still no tasks for this user, we'll check if he have in progress tasks
           {
                List<UserTask> userTasks;
                lock (taskHash)
                {
                    userTasks=taskHash[task.UserName];
                    UserTask inProgTask=userTasks.Where(ut=>ut.InProgress==true).FirstOrDefault(); 
                    if(inProgTask==null) // No inProg tasks for this user
                    {
                        task.Strted=DateTime.Now.ToString();
                        taskHash[task.UserName].Add(task);
                    }
                    else // Check if the user finished a task 
                    {
                        if (!task.InProgress) //user sent inProg =false for finished tasks
                        {
                            userTasks.Where(ut=>ut.Name==task.Name).FirstOrDefault().InProgress=false;
                            userTasks.Where(ut=>ut.Name==task.Name).FirstOrDefault().Ended=DateTime.Now.ToString();
                            bAdded=true;
                        }
                    }
                }
           }
        }
    }
}
