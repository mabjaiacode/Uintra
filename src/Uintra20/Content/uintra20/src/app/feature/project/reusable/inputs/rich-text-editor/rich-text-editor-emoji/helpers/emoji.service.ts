import { Injectable } from '@angular/core';
import { emojiList } from './emoji-list';

interface ISelection {
  index: number;
  length: number;
}

@Injectable({
  providedIn: 'root'
})
export class EmojiService {

  constructor() {
  }

  addOnTextChangeCallback(editor) {
    editor.on('text-change', (delta, source) => {
      //Create string to keep track of index
      let stringFromDelta = this.createEditorInput(editor);

      emojiList.forEach(emoji => {
        //Runs stack of methods while there are emoji shortcuts in editor
        while (stringFromDelta.includes(emoji.shortcut)) {
          //Gets index of emoji shortcut
          const index = stringFromDelta.indexOf(emoji.shortcut);
          //Delete emoji shortcut from editor
          editor.deleteText(index, emoji.shortcut.length);
          //Adds emoji on that index
          this.addEmoji(editor, emoji, index);
          //Change emoji shortcut to symbol '1' in string representation of editor's input
          stringFromDelta = stringFromDelta.slice(0, index) + '1' + stringFromDelta.slice(index + emoji.shortcut.length);
        }
      })
    });
  }

  //Creates string representaition of editor's input with '1' symbol instead of emoji
  createEditorInput(editor) {
    return editor.getContents().ops.reduce((accumulator, currentValue) => {
      if (typeof currentValue.insert === 'string') {
        //insert existing parts of string
        return accumulator + currentValue.insert;
      } else {
        //instead of images we insert '1' just to know right index
        return accumulator + '1';
      }
    }, '');
  }

  //Adds inline styles to emoji, so that they are applied outside of quill
  addStylesToImages(editor) {
    editor.container.querySelectorAll("img").forEach(img => {
      img.setAttribute('width', '20');
      img.setAttribute('height', '20');
      img.setAttribute('style', 'margin: 0 4px; vertical-align: middle');
    });
  }

  addEmoji(editor, emoji, index?) {
    //Next line inserts emoji
    editor.insertEmbed(index || this.getCursorOrContentLength(editor), 'image', emoji.src);
    //Next line puts cursor after inserted emoji
    editor.setSelection(index + 1 || this.getCursorOrContentLength(editor) + 1);
    this.addStylesToImages(editor)
  }

  getCursorOrContentLength(editor) {
    //editor.getSelection() returns object with index (index of cursor) and length (shows amount of highlighted symbols, if any)
    const cursor: ISelection = editor.getSelection();
    if (cursor) {
      //if user highlighted text we remove that
      if (cursor.length) {
        editor.deleteText(cursor.index, cursor.length);
      }
      return cursor.index;
    }
    //In case if input is not focused, using delta property of quill check if something inserted in input, if yes returns length of input otherwise returns zero
    return editor.getContents().ops.length > 1 ? editor.getLength() - 1 : 0;
  }
}