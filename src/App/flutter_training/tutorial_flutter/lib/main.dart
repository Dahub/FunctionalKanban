// Copyright 2018 The Flutter team. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

import 'package:flutter/material.dart';
import 'package:english_words/english_words.dart';
import 'Person.dart';

void main() => runApp(MyApp());

class MyApp extends StatelessWidget {
  // @override
  TextEditingController _controller = TextEditingController(text: "Initial value here");
  String _searchTerm = '';

  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Welcome to Flutter',
      home: Scaffold(
        appBar: AppBar(
          title: Text('Welcome to Flutter'),
        ),
        // body: Center(
        //   child: RandomWords(),
        // ),
        body: Container(          
          alignment: Alignment.center,
          child: 
          //  const Text('plop'),
            TextField(
              controller: _controller,
              onChanged: (String val) => _searchTerm = val,
            ),
            // Container(child: Image.asset('assets/images/zappa1.jpg', fit: BoxFit.contain)), 
            ////Person(firstName: "plop", lastName: "plffi"), // Text("Hello World"),         
        ),
        floatingActionButton : FloatingActionButton(
          child: Icon(Icons.thumb_up),
          onPressed: () => {},
        )
      ),
    );
  }
}



class RandomWords extends StatefulWidget {
  @override
  _RandomWordsState createState() => _RandomWordsState();
}

class _RandomWordsState extends State<RandomWords> {
  final _suggestions = <WordPair>[];
  final _biggerFont = TextStyle(fontSize: 18.0);

  Widget _buildSuggestions() {
    return ListView.builder(
      padding: EdgeInsets.all(16.0),
      itemBuilder: (context, i) {
        if(i.isOdd) return Divider();

        final index = i ~/ 2;
        if(index >= _suggestions.length){
          _suggestions.addAll(generateWordPairs().take(10));
        }
        return _buildRow(_suggestions[index]);
      },
    );
  }

  Widget _buildRow(WordPair wordPair){
    return ListTile(
      title: Text(
        wordPair.asPascalCase,
        style: _biggerFont
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('Startup Name Generator')
      ),
      body: _buildSuggestions(),
    );
  }
}
