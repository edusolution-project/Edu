/**
 * Copyright (c) 2020, Viet Phung
 */

// Register the plugin within the editor.
CKEDITOR.plugins.add('fillquiz', {
	// Register the icons.
	icons: 'fillquiz',

	// The plugin initialization logic goes inside this method.
	init: function (editor) {

		// Define an editor command that opens our dialog window.
		editor.addCommand('fillquiz', new CKEDITOR.dialogCommand('fillquizDialog'));
		editor.addContentsCss(this.path + 'style.css');
		// Create a toolbar button that executes the above command.
		editor.ui.addButton('fillquiz', {

			// The text part of the button (if available) and the tooltip.
			label: 'Điền từ',

			// The command to execute on click.
			command: 'fillquiz',

			// The button placement in the toolbar (toolbar group name).
			toolbar: 'insert'
		});

		if (editor.contextMenu) {

			// Add a context menu group with the Edit fillquiz item.
			editor.addMenuGroup('fillquizGroup');
			editor.addMenuItem('fillquizItem', {
				label: 'Sửa phần điền từ',
				icon: this.path + 'icons/fillquiz.png',
				command: 'fillquiz',
				group: 'fillquizGroup'
			});

			editor.contextMenu.addListener(function (element) {
				if (element.getAscendant('fillquiz', true)) {
					return { fillquizItem: CKEDITOR.TRISTATE_OFF };
				}
			});
		}

		// Register our dialog file -- this.path is the plugin folder path.
		CKEDITOR.dialog.add('fillquizDialog', this.path + 'dialogs/fillquiz.js');

	}
});