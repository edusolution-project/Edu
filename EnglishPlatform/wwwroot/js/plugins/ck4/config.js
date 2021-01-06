/**
 * @license Copyright (c) 2003-2019, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see https://ckeditor.com/legal/ckeditor-oss-license
 */

CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here. For example:
	config.language = 'vi';
	// config.uiColor = '#AADC6E';

	config.toolbarGroups = [
		{ name: 'document', groups: ['mode', 'document', 'doctools'] },
		{ name: 'clipboard', groups: ['clipboard', 'undo'] },
		//{ name: 'editing', groups: ['find', 'selection', 'spellchecker', 'editing'] },
		//{ name: 'forms', groups: ['forms'] },
		//'/',
		{ name: 'basicstyles', groups: ['basicstyles', 'cleanup'] },
		{ name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi', 'paragraph'] },
		{ name: 'links', groups: ['links'] },
		{ name: 'insert', groups: ['insert'] },
		//'/',
		{ name: 'styles', groups: ['styles'] },
		{ name: 'colors', groups: ['colors'] },
		{ name: 'tools', groups: ['tools'] },
		{ name: 'others', groups: ['others'] },
		{ name: 'about', groups: ['about'] }
	];

	config.removeButtons = 'CreateDiv,Language,Anchor,Flash,Smiley,About,Print,NewPage,Save,TextField,Font,FontSize,Subscript,Superscript';
    
    config.removeDialogTabs = 'image:advanced;link:advanced;textfield';
	config.extraPlugins = 'uploadimage';
	config.extraPlugins = 'youtube';
	config.imageUploadUrl = '/Home/UploadImage';
	//config.extraAllowedContent = 'fillquiz';
};
