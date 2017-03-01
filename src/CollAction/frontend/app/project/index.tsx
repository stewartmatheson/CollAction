import * as React from "react";
import * as ReactDOM from "react-dom";
import FindProject from "./FindProject";
import LandscapeCarousel from "./LandscapeCarousel";
import "./style.scss";

function renderComponentIf(component, element) {
  if (element) {
    ReactDOM.render(component, element);
  }
}

const startProjectSteps = [
  {
    name: "Register",
    text: "Complete the registration form here [hyperlink to form] in full."
  },
  {
    name: "Assessment",
    text: "CollAction judges if your project meets the criteria on this page. If necessary, CollAction contacts you to discuss/clarify."
  },
  {
    name: "Placement",
    text: "Upon approval, the project will be placed on collaction.org.",
  },
  {
    name: "Campaign",
    text: "Campaign to reach your target."
  },
  {
    name: "Action",
    text: "When the target is met at the time of the deadline, all supporters will take action."
  },
  {
    name: "Measuring impact",
    text: "After the action period, measure how many people took part in the project and share this with the CollAction team. This allows us to project your project, measure the impact of CollAction, and inspire other people to start and support projects."
  },
];

renderComponentIf(
  <LandscapeCarousel items={startProjectSteps} />,
  document.getElementById("project-create-landscape-carousel")
);

renderComponentIf(
  <FindProject />,
  document.getElementById("project-controller")
);