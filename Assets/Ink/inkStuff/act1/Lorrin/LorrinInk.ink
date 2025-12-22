// ============================================
// #region ACT ONE
// ============================================

VAR hasReadDiary = false


=== LorrinWalkTheLine ===
Ohh, you finaly awoken! [M1 to continue]
Alright, let’s see if we can get you on your feet.
Why dont you walk towards me, but take it slow now, it aint a race!
[w/a/s/d to move]
[L to open Questlog]
[E to interact, shift to dash] #startquest:Talk The Line
-> END



=== LorrinPickinWildBerries ===
Look who’s awake.
I was doing my routes when I saw something floating in the water.
Turns out that sometething was you. and i was hoping for a paimon
gave me a scare, but I’m glad you made it.
You look hungry.
I got some appleberries growing just up northwest.
Theyll bring life back to ya! (literally) #startquest:Pickin Wild Berries

- (options)

* "Who are you?"
-> backstory

* {hasReadDiary} "Who are you mentioning in your diary?"
-> DiaryResponse

+ "See you in a bit."-> DONE

 // Exit from the main menu

=== backstory ===
Name’s Lorrin.
Welcome to my little homestead.
When I’m not writing in my diary or tending the garden,
I’m down at the beach scavenging and fishing.
I like the quiet.
A lot simpler than dealing with the Circles.

** "Circles?"
-> CirclesResponse

** "I had another question."
-> LorrinPickinWildBerries.options

** "I have to get going.
-> DONE // Absolute quit from the entire story.

=== CirclesResponse ===
You’re not from around here, are you?
There are four Circles running things now.

First, the Iron Circle: a military hammer that wants magic wiped out completely.
Their hatred burns hotter than their forges.

Then the Jade Circle: druids and monsters who swore to protect magical beings after the Sundering.

The Lapis Circle is a greedy bunch of merchants.
Under their king, the rich get richer, yet the poor stays poor.

And… there are whispers of a fourth Circle... 
Probably just Campfire stories. 
reminds me i need to get some new firewood for the fire.

Anyway… go get those berries.   
You still look rough.

* Okay.
-> LorrinPickinWildBerries.options

+ Wait, let me ask something else.
-> LorrinPickinWildBerries.options

+ I have to get going.

-> DONE // Absolute quit from the entire story.



==ToTheBeach== 

Aren’t they delicious?
Keep a few on you — they might just save your hide.

Now that you’re feeling better, there’s something else 
we need to talk about.
Right when I found you, a mighty earthquake hit.
I grabbed you, but I couldn’t get to a bag 
that I’m guessing was yours.

The quake sealed the path behind us, but… 
lucky for us, I always keep a few explosives handy. kablooey!
There — I’ve set them up. Just give it a good hit.

The beach is to the left from here.
Be cautious. You probably won’t be alone out there.

#startquest:To The Beach
#spawnQuestObject:Barrel1
- (options)

* {hasReadDiary} "Who are you mentioning in your diary?"
-> DiaryResponse

*Well, let's go to the beach, then. (DONE)
-> END

*See you later. (DONE)
-> END



==WildThings==
Told you the beach are dangerous. 
But im happy i didnt had to pull you out there a second time.
That glow… I haven’t seen anything like it since the Sundering.
You can’t stay here with that.
The Jade Circle needs to see it at once.
I’ve heard they’ve taken shelter in a cave to the east, 
after their grove burned down.
Can’t imagine what that did to them… losing their home like that.


Go. Now. Before someone else senses it.
#startquest:Wild Things
-> DONE


==Diary==
~ hasReadDiary = true
This and that

~ hasReadDiary = true

-> DONE

==DiaryResponse==
Welll...


-> END

// ============================================
// #endregion ACT ONE
// ============================================






// #endregion ACT ONE
// ============================================







