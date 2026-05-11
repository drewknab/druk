---
layout: post
title: Elixir Three Years Later
author: Drew
tags: development, elixir
published: 2025-02-01
---

In February, 2022, my team had an idea to build our flagship product in a language that none of us knew. It's now three years, many projects, and many clients later. 

<!--more-->

## How did I get here?

When I started, the project had already existed for about one-and-a-half years with very little work that was UAT ready, let alone production.
We had to make some decisions. We gathered in the small room, which we semi-endearingly called The Dev Cave on account of it's bare brick wall and general cave-like atmosphere,
and tried to figure out the direction we needed to go.
There were a couple of extremely bare bones prototypes written in JavaScript and some undercooked .NET applications that were being written by an offshore consulting team.
In all fairness, they wrote precisely what was in the limited spec, that the spec no longer resembled the real-world needs of our startup or the needs of customers was hardly their fault.

The .NET application were too complex and too far into development to abandon. We made the decision to keep and modify them to the changing business needs.
We abandoned the old prototypes for new prototypes.
One of those was an abstraction that stood between the telephony, NLP, and banking software. The design and scope changed about a half dozen times in three weeks.
Should we write it in JavaScript? Python? F# (my personal choice)? Someone, who remains anonymous, suggested Elixir.

Elixir had been at the periphery of my professional explorations.
Of the four members of the team at the time I was the most hesitant of making the leap.
Only one of us had any experience with it at all.
We didn't know how to structure a critical piece of software out of it.
We didn't know what kinds of gotchas we were walking into.
We didn't know what performant Elixir even looked like, but three of us (myself included) were functional programming junkies.

My main arguments against incorporating it into the organization were:
1. We didn't actually know that much about telephony
2. We don't actually know that much about Elixir
3. We do actually know about NLP
4. We will be learning two distinct technologies on the fly
5. Any new software engineer added to the team will be where we were when the project started

With all that established, we still dove head-first into the unknown.

## What about Elixir?
Elixir, if you're unaware, is a functional programming language built on top of Erlang running on the BEAM virtual machine. It uses many of the same abstractions Erlang established to build what are styled as "distributed, fault-tolerant applications". This means that it's a language filled to the brim with arcane abstractions and Erlang Open Telecom Protocol (OTP) magic.

More practically, it is a dynamically typed functional language with a syntax and ethos that borrows heavily from Ruby. It has a robust standard library that tries to reduce the necessity for big utility function modules. In addition, it also borrows Ruby's (and Erlang's) penchant for metaprogramming via macros. All these features mean the learning curve is unique.

The basics of the language are fairly simple to learn. The major packages, like Phoenix and Ecto, are featureful and make the process of building software for the Web a breeze. Past this though, the otherwise hidden depths start to emerge.
