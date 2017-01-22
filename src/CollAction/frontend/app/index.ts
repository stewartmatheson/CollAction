import "./header/style.scss"
import "./footer/style.scss"

import ProjectFormBoom from './project/form';

window['CollAction'] = {
  forms: {
    Project : { boom: ProjectFormBoom },
  }
}