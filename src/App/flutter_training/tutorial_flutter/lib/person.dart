import 'package:flutter/material.dart';

class Person extends StatelessWidget {
  final String firstName;
  final String lastName;
  Person({this.firstName, this.lastName}) {}
  Widget build(BuildContext context){
    return Container(child: Text('$firstName $lastName'));
  }
}