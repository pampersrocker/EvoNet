# EvoNet

An evolution simulation in .Net

[![Stories in Ready](https://badge.waffle.io/pampersrocker/EvoNet.svg?label=ready&title=Ready)](http://waffle.io/pampersrocker/EvoNet)
[![Stories in Progress](https://badge.waffle.io/pampersrocker/EvoNet.svg?label=In%20Progress&title=In%20Progress)](http://waffle.io/pampersrocker/EvoNet)

## Overview

For the base simulation a TileMap is generated which seperates the tilemap into land and water.
Land is fertile and grass/food can grow, which grow speed is dependent in its neighboring tiles.

![TileMap](docs/EvoNet.gif)

On this world live creatues, which taught themselves to move, eat and reproduce.

## Project Setup

This project uses [XNA 4.0 Refresh](https://blogs.msdn.microsoft.com/uk_faculty_connection/2015/11/02/installing-xna-with-visual-studio-2015/).

In addition some libraries are referenced using [NuGet](https://www.nuget.org/).

Main development is done using Visual Studio 2015, other platforms might work but are unsupported and not tested.

## Management

Issue tracking and work distribution is done using [waffle.io](https://waffle.io/pampersrocker/EvoNet)
