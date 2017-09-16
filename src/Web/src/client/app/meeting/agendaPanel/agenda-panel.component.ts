import {
    Component,
    OnInit,
    Input,
    EventEmitter,
    Output,
    OnChanges,
    SimpleChanges
} from '@angular/core';
import {
    MeetingModel
} from "../../shared/models/meetingModel";
import {
    MeetingAgenda
} from '../../shared/models/meetingAgenda';
declare let $: any;
@Component({
    moduleId: module.id,
    selector: 'sd-agenda-panel',
    templateUrl: 'agenda-panel.component.html',
    styleUrls: ['agenda-panel.component.css']
})
export class AgendaPanelComponent implements OnInit, OnChanges {
    Name:string;
    QuickTopic: string;
    @Input() Id: string;
    @Input() Meeting : MeetingModel;
    @Output() Click = new EventEmitter();
    @Output() Topic = new EventEmitter<MeetingAgenda>();
    private createId(): any {
        return `${this.createidsection()}-${this.createidsection()}-${this.createidsection()}-${this.createidsection()}` +
               `${this.createidsection()}-${this.createidsection()}-${this.createidsection()}-${this.createidsection()}`;
    }
    private createidsection() {
        return Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1);
    }
    public ngOnChanges(changes : SimpleChanges) {
        console.log(changes);
    }
    public ngOnInit() {
        if (!this.Id) {
            this.Name = this.createId();
        }else {
            this.Name = this.Id;
        }
        if(!this.Meeting.MeetingAgendaCollection) {
            this.Meeting.MeetingAgendaCollection = [];
        }
    }
    public AddTopic($event : any) {
        if(!this.Meeting.MeetingAgendaCollection) {
            this.Meeting.MeetingAgendaCollection = [];
        }
        let quickTopic = new MeetingAgenda();
        quickTopic.AgendaHeading = this.QuickTopic;
        quickTopic.Duration = '00:00';
        this.Meeting.MeetingAgendaCollection.push(quickTopic);
    }
    public click():void {
        this.Click.emit();
    }
    public allowDrop(ev : any) {
        ev.preventDefault();
    }
    public drag(ev : any) {
        ev.dataTransfer.setData("text", ev.target.id);
    }
    public drop(ev : any) {
        ev.preventDefault();
        var data = ev.dataTransfer.getData("text");
        ev.target.appendChild(document.getElementById(data));
    }
    public SelectTopic(agenda : MeetingAgenda) {
        this.Topic.emit(agenda);
    }
}
