import { KeyValue } from "../interfaces/KeyValue";
import { ContentStatistics } from "./models/Content";
import { PropertyStatistics } from "./models/Property";
import { UserStatistics } from "./models/Users";
import { Chart } from 'chart.js';
import { Alerts, MediaObject, MediaService } from "../hood";

export class MediaController {

    constructor() {
        this.manage();
    }

    list: HTMLElement;
    service: MediaService;

    manage(): void {
        this.list = document.getElementById('media-list');
        this.service = new MediaService(this.list, {
            action: 'show',
            onAction: function (this: MediaController, sender: HTMLElement, mediaObject: MediaObject) {
                Alerts.log(`Showing media object id ${mediaObject.id} - ${mediaObject.filename}`);
            }.bind(this),
            onListLoad: (sender: HTMLElement) => {
                Alerts.log('Commencing media list fetch.');
            },
            onError: (jqXHR: any, textStatus: any, errorThrown: any) => {
                Alerts.log(`Error loading media list: ${textStatus}`);
            },
            onListRender: (sender: HTMLElement, data: string) => {
                Alerts.log('Fetched media list data.');
                return data;
            },
            onListComplete: (sender: HTMLElement, data: string) => {
                Alerts.log('Finished loading media list... now attach the media actions to the things.', 'warning');
            }
        });
    }
}