import * as React from "react";
import renderComponentIf from "../global/renderComponentIf";

interface ITeamMember {
  name: string;
  role: string;
}

interface IMeetTheTeamProps {
  teamMembers: ITeamMember[];
}

interface IMeetTheTeamState {
  currentIndex: number;
}

const TeamMemberThumb = ({ teamMember }) => {
  const imageSrc = `/images/team/${teamMember.name}.jpg`;
  return (
    <div className="team-member">
      <div>
        <img src={imageSrc} />
      </div>
      <div>
        {teamMember.name}
      </div>
      <div>
        Team Member
      </div>
    </div>
  );
}

const NavigationDots = ({ currentIndex, length}) => {
  const dots = [...Array(length).keys()].map((index) => {
    if (index === currentIndex) {
      return <div className="dot dot-active" key={index} />;
    }
    return <div className="dot" key={index} />;
  });

  return (
    <div id="dot-container">
      { dots }
    </div>
  );
}

export default class MeetTheTeam extends React.Component<IMeetTheTeamProps, IMeetTheTeamState> {

  constructor (props) {
    super(props);
    this.state = { currentIndex: 0 };
  }

  render () {
    return (
      <div>
        <div id="team-member-container">
          {this.props.teamMembers.map(teamMember => <TeamMemberThumb key={teamMember.name} teamMember={teamMember} />)}
        </div>
        <NavigationDots currentIndex={this.state.currentIndex} length={this.props.teamMembers.length}/>
      </div>
    )
  }
}

const teamMembers: ITeamMember[] = [
  {
    name: "Brankele Frank",
    role: "",
  },
  {
    name: "Daniela Becker",
    role: "",
  },
  {
    name: "Edoardo Felici",
    role: "",
  },
  {
    name: "Joost Riphagen",
    role: "",
  },
  {
    name: "Marc Beermann",
    role: "Developer",
  },
  {
    name: "Maria Gomez",
    role: "",
  },
  {
    name: "Martijn de Haan",
    role: "",
  },
  {
    name: "Melchior Jong",
    role: "",
  },
  {
    name: "Mina Yao",
    role: "Developer",
  },
  {
    name: "Ron van den Akker",
    role: "",
  },
  {
    name: "Spencer Heijnen",
    role: "",
  },
  {
    name: "Stewart Matheson",
    role: "Developer",
  },
  {
    name: "Tim Stokman",
    role: "Developer",
  },
  {
    name: "Vivian Vos",
    role: "",
  },
];

renderComponentIf(
  <MeetTheTeam teamMembers={teamMembers} />, 
  document.getElementById("meet-the-team-widget")
);
