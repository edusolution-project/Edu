/**
 * Copyright (c) 2020, Viet Phung
 */

// Our dialog definition.
CKEDITOR.dialog.add('fillquizDialog', function (editor) {
    return {

        // Basic properties of the dialog window: title, minimum size.
        title: 'Fillquiz Properties',
        minWidth: 400,
        minHeight: 200,

        // Dialog window content definition.
        contents: [
            {
                // Definition of the Basic Settings dialog tab (page).
                id: 'tab-fillquiz',
                label: 'FillQuiz Settings',

                // The tab content.
                elements: [
                    {
                        // Text input field for the Fillquiz Placeholder text.
                        type: 'text',
                        id: 'quizPlc',
                        label: 'Placeholder',

                        // Validation checking whether the field is not empty.
                        validate: CKEDITOR.dialog.validate.notEmpty("Placeholder field cannot be empty."),

                        // Called by the main setupContent method call on dialog initialization.
                        setup: function (element) {
                            var childInp = element.find("input");
                            if (childInp.$.length > 0) {
                                this.setValue(childInp.$[0].getAttribute("plc"));
                            }
                            else {
                                this.setValue(element.getText());
                            }
                        },

                        // Called by the main commitContent method call on dialog confirmation.
                        commit: function (element) {
                            var childInp = element.find("input");
                            if (childInp.$.length == 0) {
                                element.setText("");
                                var childInp = editor.document.createElement('input');
                                childInp.setAttribute("type", "text");
                                childInp.setAttribute("plc", this.getValue());
                                childInp.setAttribute("value", this.getValue());
                                childInp.setAttribute("disabled", "disabled");
                                childInp.setAttribute("class", "fillquiz");
                                element.append(childInp);
                            }
                            else {
                                childInp.$[0].setAttribute("plc", this.getValue());
                                childInp.$[0].setAttribute("value", this.getValue());
                            }
                        }
                    },
                    {
                        // Text input field for the Fillquiz text.
                        type: 'text',
                        id: 'quizAnswer',
                        label: 'Answer',

                        // Validation checking whether the field is not empty.
                        validate: CKEDITOR.dialog.validate.notEmpty("Answer field cannot be empty."),

                        // Called by the main setupContent method call on dialog initialization.
                        setup: function (element) {
                            var childInp = element.find("input");
                            if (childInp.$.length > 0) {
                                this.setValue(childInp.$[0].getAttribute("ans"));
                            }
                            else {
                                this.setValue(element.getText());
                            }
                        },

                        // Called by the main commitContent method call on dialog confirmation.
                        commit: function (element) {
                            var childInp = element.find("input");
                            if (childInp.$.length == 0) {
                                element.setText("");
                                var childInp = editor.document.createElement('input');
                                childInp.setAttribute("type", "text");
                                childInp.setAttribute("ans", this.getValue());
                                var text = "(" + this.getValue() + ")";
                                var i = text.length;
                                var space = 0;
                                while (i--) { if (text.charAt(i) == ' ') space++; }

                                childInp.$[0].setAttribute("value", text);
                                childInp.$[0].setAttribute("size", text.length - space);
                                childInp.setAttribute("disabled", "disabled");
                                childInp.setAttribute("class", "fillquiz");
                                element.append(childInp);
                            }
                            else {
                                childInp.$[0].setAttribute("ans", this.getValue());
                                var text = childInp.$[0].getAttribute("value") + " (" + this.getValue() + ")";
                                var i = text.length;
                                var space = 0;
                                while (i--) { if (text.charAt(i) == ' ') space++; }

                                childInp.$[0].setAttribute("value", text);
                                childInp.$[0].setAttribute("size", text.length - space);
                            }
                        }
                    },
                    {
                        // Text input field for the Fillquiz title (explanation).
                        type: 'text',
                        id: 'quizExp',
                        label: 'Explanation',
                        validate: CKEDITOR.dialog.validate.notEmpty("Explanation field cannot be empty."),

                        // Called by the main setupContent method call on dialog initialization.
                        setup: function (element) {
                            this.setValue(element.getAttribute("title"));
                        },

                        // Called by the main commitContent method call on dialog confirmation.
                        commit: function (element) {
                            element.setAttribute("title", this.getValue());
                        }
                    }
                ]
            }
        ],

        // Invoked when the dialog is loaded.
        onShow: function () {
            // Get the selection from the editor.
            var selection = editor.getSelection();

            // Get the element at the start of the selection.
            var element = selection.getStartElement();

            // Get the <fillquiz> element closest to the selection, if it exists.
            if (element)
                element = element.getAscendant('fillquiz', true);

            // Create a new <fillquiz> element if it does not exist.
            if (!element || element.getName() != 'fillquiz') {
                {
                    element = editor.document.createElement('fillquiz');
                }

                // Flag the insertion mode for later use.
                this.insertMode = true;
            }
            else {
                this.insertMode = false;
            }

            // Store the reference to the <fillquiz> element in an internal property, for later use.
            this.element = element;

            // Invoke the setup methods of all dialog window elements, so they can load the element attributes.
            if (!this.insertMode)
                this.setupContent(this.element);
        },

        // This method is invoked once a user clicks the OK button, confirming the dialog.
        onOk: function () {
            //var dialog = this;
            // Create a new <fillquiz> element.
            var fillquiz = this.element;
            // Invoke the commit methods of all dialog window elements, so the <fillquiz> element gets modified.
            this.commitContent(fillquiz);

            // Finally, if in insert mode, insert the element into the editor at the caret position.
            if (this.insertMode) {
                editor.insertElement(fillquiz);
                //debugger;

                //var quizPlc = dialog.getContentElement('tab-fillquiz', 'quizPlc').getValue();
                //var quizAnswer = dialog.getContentElement('tab-fillquiz', 'quizAnswer').getValue();
                //var quizExp = dialog.getContentElement('tab-fillquiz', 'quizExp').getValue();
            }
        }
    };
});