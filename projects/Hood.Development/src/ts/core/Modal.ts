﻿import { Modal } from "bootstrap";
import { Inline } from "./Inline";

export interface ModalOptions {

    /**
     * Called before the data is fetched.
     */
    onLoad?: (sender: HTMLElement) => void;

    /**
     * Called before the fetched HTML is rendered to the list. Must return the data back to datalist to render.
     */
    onRender?: (sender: HTMLElement, html: string) => string;

    /**
     * Called when loading and rendering is complete.
     */
    onComplete?: (sender: HTMLElement) => void;

    /**
     * Called when loading and rendering is complete.
     */
    onError?: (jqXHR: any, textStatus: any, errorThrown: any) => void;

    closePrevious?: boolean;
}


export class ModalController {
    element: HTMLElement;
    modal: Modal;
    options: ModalOptions = {
        closePrevious: true
    }

    constructor(options: ModalOptions) {
        this.options = { ...this.options, ...options };
    }

    show(url: string | URL, sender: HTMLElement) {
        if (this.options.onLoad) {
            this.options.onLoad(this.element);
        }

        $.get(url as string, function (data: string) {

            if (this.modal && this.options.closePrevious) {
                this.close();
            }

            if (this.options.onRender) {
                data = this.options.onRender(this.element, data);
            }

            this.element = this.createElementFromHTML(data);
            this.element.classList.add('hood-inline-modal');

            $('body').append(this.element);
            this.modal = new Modal(this.element, {});
            this.modal.show();

            // Workaround for sweetalert popups.
            this.element.addEventListener('shown.bs.modal', function (this: ModalController) {
                $(document).off('focusin.modal');
            }.bind(this));
            this.element.addEventListener('hidden.bs.modal', function (this: ModalController) {
                this.close();
            }.bind(this));

            if (this.options.onComplete) {
                this.options.onComplete(this.element);
            }
        }.bind(this))
            .fail(this.options.onError ?? Inline.handleError);
    }

    close() {
        if (this.modal) {
            this.modal.hide();
            this.modal.dispose();
            this.element.remove();
        }
    }

    createElementFromHTML(htmlString: string): HTMLElement {
        var div = document.createElement('div');
        div.innerHTML = htmlString.trim();

        // Change this to div.childNodes to support multiple top-level nodes
        return div.firstChild as HTMLElement;
    }
}