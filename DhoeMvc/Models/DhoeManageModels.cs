using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace DhoeMvc.Models.DhoeManageModels
{
  public class StudentsData
  {
    public string no { get; set; }
    public string Name { get; set; }
    public string DataType { get; set; }
    public string Kind { get; set; }
    public List<string> ExperienceList { get; set; }
    public List<string> ResearchList { get; set; }
    //public List<StudentsExperience> ExperienceList { get; set; }
    //public List<StudentsResearch> ResearchList { get; set; }
  }

  public class StudentsExperience
  {
    public string desc { get; set; }
    //public DateTime StartDate { get; set; }
    //public DateTime EndDate { get; set; }
  }


  public class StudentsResearch
  {
    public string desc { get; set; }
    //public DateTime StartDate { get; set; }
    //public DateTime EndDate { get; set; }
  }
}