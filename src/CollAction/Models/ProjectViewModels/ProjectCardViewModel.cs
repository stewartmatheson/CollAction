using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CollAction.Models
{

    /**
     * A view model that will display each project card.
     */
    public class ProjectCardViewModel
    {
      public string Name { get; set; }
      public string Color { 
          get { return "#cdcdcd"; }
      }
      
      public static implicit operator ProjectCardViewModel(Project p) {
          ProjectCardViewModel pcvm = new ProjectCardViewModel { 
            Name = p.Name
          };
          return pcvm;
      }
    }

}