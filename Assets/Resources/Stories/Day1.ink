EXTERNAL ChangeScene(sceneType)
EXTERNAL FadeBackground(fileName, duration)
EXTERNAL Delay(duration)
EXTERNAL PlayAudio(fileName)
EXTERNAL LoopAudio(fileName)
EXTERNAL SetAudioVolume(fileName, volume)
EXTERNAL FadeAudioVolume(fileName, volume, duration)
EXTERNAL StopAudio(fileName)

~ ChangeScene("Title")

Chapter 1 Day 1\\n<>
The Long Awaited Whistle

~ ChangeScene("Subtitle")

It's been years since the civil war ended, but Alaska's scars linger along with the rest of America. For us, cities and villages lie half-buried under heavy snow, their chimneys cold, while a new capital stands revitalized, a distant beacon of order. A new railroad snakes through the wild to reconnect the isolated and rebuild the broken. At first glance, these trains might seem a relic of the past, but are in fact our present. However remnants of the older world persist: fragments of high-speed rails, social media echoes, and sports magazines depicting large crowds gathering to see their favorite athletes. The days of bustling malls and divisive politics are gone, replaced by a fragile unity born of necessity. #title=Year: 2050:

We're all under one sky, no matter the noise below. A sound from from the sky might remind us we're not as divided as we think.” -Olga Orlov

~ ChangeScene("Blank")
~ FadeBackground("villagers_color", 5)
~ LoopAudio("dragonwind")
~ SetAudioVolume("dragonwind", 1)
~ Delay(5)
~ ChangeScene("Dialog")

An arctic village clings to life like a dying ember, its snow-choked homes casting no smoke. The sky hangs dull gray, swallowing shadows. Thin figures stand in the drifts, bundled in layers piled high against the artic cold, their breath clouding in the biting air.

A mournful wind cuts through the silence, broken only by occasional distant chatter.

~ SetAudioVolume("dragonwind", 0.2)

[Looking down, wringing her hands. Her voice trembles, eyes scanning the white horizon.] #speaker=Sara #center_portraits=sara_half

When are they coming? #speaker=Sara #center_portraits=sara_half

[Shivering, clutching a thin blanket, tugging at two nearby adults. His lips are chapped from the cold.] #speaker=Tim #center_portraits=tim_half

Are they really coming? #speaker=Tim #center_portraits=tim_half

[Kneeling, forcing a smile, though his eyes betray doubt.] #speaker=Frank #center_portraits=frank_half

Don't worry, Tim, I'm sure they'll come. [His hand rests on Tim's head.] #speaker=Frank #center_portraits=frank_half

[Arms crossed, staring at the ground, voice flat and resigned.] #speaker=Bobby #center_portraits=bobby_half

Hmph. The capital starts the mess, then takes its sweet time fixing it. Bet they've forgotten us. #speaker=Bobby #center_portraits=bobby_half

[Hunched slightly, shivering to generate any warmth possible.] #speaker=Mara #center_portraits=mara_half

We're out of supplies and options. All we can do is stay optimistic and wait. #speaker=Mara #center_portraits=mara_half

[Snapping, fists clenched.] #speaker=Bobby #center_portraits=bobby_half

It's this damn war's fault! We didn't ask for it, yet we're the ones suffering. One idiot glares at another, and next thing you know, we're all buried in snow. #speaker=Bobby #center_portraits=bobby_half

His words echo briefly, then fade into the drifts.

[Huddling closer to Sara, his small voice lost in the wind.] #speaker=Tim #center_portraits=tim_half

I'm cold… and hungry… #speaker=Tim #center_portraits=tim_half

[Standing tall, voice booming despite his frail frame.] #speaker=Arthur #center_portraits=arthur

Everyone! Let's gather and pray for a new life, for the seasons to turn again, for a sign from above to carry us forward! #speaker=Arthur #center_portraits=arthur

He raises his hands, inviting the villagers closer. Villagers shuffle closer, forming a rough circle. Hands clasp, lips move silently as snow dances around them.

~ SetAudioVolume("dragonwind", 1)

The wind howls louder, drowning murmurs. Then… silence... A deafening void swallows the village.

~ SetAudioVolume("dragonwind", 0.2)

[Looking over the crowd with warm, steady eyes.] #speaker=Arthur #center_portraits=arthur

My friends, my family, set aside your burdens for a moment. I know we're tired and scared. The days are long, and help feels far. But we are not alone, not now, not ever. The Lord is with us, calling us to stand together. Will you pray with me? #speaker=Arthur #center_portraits=arthur

Villagers nod, some clasping hands, others bowing heads. Arthur raises his chin, signaling the start of the prayer.

Heavenly Father, in this hour of trial, we turn to You, our rock and refuge. Grant us strength to endure, courage to face our fears, and faith to trust Your plan. Surround us with Your love, unite us in hope, and guide us through this storm. May Your peace fill our hearts as we await Your deliverance. Amen. #speaker=Arthur #center_portraits=arthur

~ ChangeScene("Blank")
~ FadeBackground("snowy_village_color", 0.5)
~ StopAudio("dragonwind")
~ LoopAudio("cold_strong_wind")
~ Delay(10)
~ FadeAudioVolume("cold_strong_wind", 0, 5)
~ Delay(15)
~ ChangeScene("Dialog")
~ StopAudio("cold_strong_wind")

The prayer fades, buried in silence. A cold gust sweeps through, as if the sky itself presses down on the world.

~ ChangeScene("Blank")
~ FadeBackground("black", 5)
~ Delay(5)
~ PlayAudio("amazing_grace")
// ~ Delay(45)
~ Delay(5)
~ FadeBackground("snowy_village_color", 5)
~ Delay(5)
~ StopAudio("amazing_grace")
~ ChangeScene("RhythmGame")

The trumpet's melody, pure and resonant, rolls across the sky like a beacon, rising and falling with haunting clarity.\\n\\n<>
It's not the train's rumble that reaches the village first, but this sacred tune, amplified through the trumpet spreading the clouds, carrying hope on every note.\\n\\n<>
Villagers stir, faces lifting, eyes widening as the music grows vibrant, each tune a promise of rescue, a sound from the sky.

~ ChangeScene("Blank")
~ FadeBackground("black", 5)
~ Delay(5)
~ FadeBackground("cover_train_tender_color", 5)
~ LoopAudio("steam_train_with_whistle")
~ Delay(5)
~ ChangeScene("Dialog")

A pinprick of light flickers along the tracks, the train's headlamp cutting through the mist. The low, rhythmic chug of the steam engine creeps in, a soft pulse growing into a deep roar that vibrates through the frozen earth, harmonizing with the trumpet's flair.

The train emerges from the fog, its dark silhouette massive against the white expanse, steam billowing in silvery plumes that catch the weak sunlight. Villagers clutch one another, some weeping, others smiling, their fear thawing in the moment's warmth. The ground trembles as the iron beast closes the distance, its promise of salvation deafening and radiant with hope.

~ StopAudio("steam_train_with_whistle")
~ ChangeScene("Blank")
~ FadeBackground("black", 5)
~ Delay(5)
~ ChangeScene("Title")

Chapter 1 Day 1\\n<>
Arrival of Trumpeter

~ ChangeScene("Blank")
~ FadeBackground("cover_train_tender_color", 5)
~ PlayAudio("steam_train_stopping")
~ Delay(10)
~ ChangeScene("Dialog")

The train cuts through snowdrifts, slowing but still thundering, its engine shaking the village. It halts with a screech of rails, steam hissing as the passenger car door slides open.

~ FadeBackground("snowy_village_color", 0.1)

[Standing firm, coat crisp, expression calm but eyes warm. A trumpet neatly strap to her coat.] #speaker=Heidi #center_portraits=heidi_half

I'm sorry for the delay. Let us travel together, toward home. #speaker=Heidi #center_portraits=heidi_half

Her voice is steady, kind, but measured, cutting through the winter silence.

The passenger car door flings open, revealing three crew members. Two more trot down from the locomotive to meet the new guests.

Tim and Sara dash toward Heidi, their faces bright with renewed energy.

[Walking up to Heidi, relieved.] #speaker=Arthur #left_portraits=heidi_half #right_portraits=arthur

I'm so glad you came. We feared help wouldn't arrive. #speaker=Arthur #left_portraits=heidi_half #right_portraits=arthur

I apologize again for the delay. It takes longer than we'd like to complete mission briefings and secure supplies for such a journey. We'd prefer to depart at a moment's notice, but that's rarely possible. #speaker=Heidi #left_portraits=heidi_half #right_portraits=arthur

~ PlayAudio("snowy_footstep")

[Approaching grumpily.] #speaker=Bobby #left_portraits=heidi_half #right_portraits=bobby_half

You took your damn time, as far as I'm concerned. #speaker=Bobby #left_portraits=heidi_half #right_portraits=bobby_half

[Apologetic] #speaker=Heidi #left_portraits=heidi_half #right_portraits=bobby_half

My sincerest apology once more. Protocol caused some delay. We— #speaker=Heidi #left_portraits=heidi_half #right_portraits=bobby_half

[Annoyed] #speaker=Bobby #left_portraits=heidi_half #right_portraits=bobby_half

Why didn't you just wait until Sunday to come. At least on Sunday you would have had an excuse to take your damn sweet time #speaker=Bobby #left_portraits=heidi_half #right_portraits=bobby_half

[With dry humor, unfazed.] #speaker=Heidi #left_portraits=heidi_half #right_portraits=bobby_half

\*\*Sigh\*\*, Your opinion has been noted sir. Next time, we'll make sure we arrive Sunday. If you want, you can wait while everyone else board, and we'll come back Sunday. Perhaps even add a cherry on top of your Sunday when we arrive. #speaker=Heidi #left_portraits=heidi_half #right_portraits=bobby_half

[Red-faced with anger.] #speaker=Bobby #left_portraits=heidi_half #right_portraits=bobby_half

You! #speaker=Bobby #left_portraits=heidi_half #right_portraits=bobby_half

Heidi meets his glare with a calm, emotionless stare, unbefitting an experienced conductor meant to lead.

[Fuming.] #speaker=Bobby #left_portraits=heidi_half #right_portraits=bobby_half

You think you're tough, don't you? Coming from your cozy capital with everything you need. You wouldn't understand enduring the aftermath of your mess. #speaker=Bobby #left_portraits=heidi_half #right_portraits=bobby_half

[Snarky but measured.] #speaker=Heidi #left_portraits=heidi_half #right_portraits=bobby_half

You're right, sir. I don't understand your struggles. Which is why I'm glad I'm not you. #speaker=Heidi #left_portraits=heidi_half #right_portraits=bobby_half

You! #speaker=Bobby #left_portraits=heidi_half #right_portraits=bobby_half

Heidi's crew watches, worried by her dry retort, a trait well-known in the capital but rarely called out. She seems oblivious to its effect. #speaker=Heidi #left_portraits=heidi_half #right_portraits=bobby_half

[Grabbing Bobby to calm him.] #speaker=Frank #left_portraits=heidi_half #right_portraits=bobby_half,frank_half

Come on, Bobby, focus. This isn't the time. #speaker=Frank #left_portraits=heidi_half #right_portraits=bobby_half,frank_half

[To Heidi, with a sincere look.] #speaker=Frank #left_portraits=heidi_half #right_portraits=bobby_half,frank_half

I'm sorry about him. Bobby's rough around the edges, but he's trustworthy and cares deeply. We've been friends for years, his heart doesn't match his gruffness. My name is Frank by the way. #speaker=Frank #left_portraits=heidi_half #right_portraits=bobby_half,frank_half

[Nodding, understanding.] #speaker=Heidi #left_portraits=heidi_half #right_portraits=bobby_half,frank_half

Thank you, Mr. Frank. I understand more clearly now, Bobby's like a polar bear, tough outside, soft inside. #speaker=Heidi #left_portraits=heidi_half #right_portraits=bobby_half,frank_half

Uh… #speaker=Frank #left_portraits=heidi_half #right_portraits=bobby_half,frank_half

What the— #speaker=Bobby #left_portraits=heidi_half #right_portraits=bobby_half,frank_half

[Exasperated.] #speaker=Mara #left_portraits=heidi_half #right_portraits=mara_half

Can we stop bickering and get on board? Focus on helping everyone settle instead of arguing and bothering people trying to help us Bobby. #speaker=Mara #left_portraits=heidi_half #right_portraits=mara_half

[To Bobby, de-escalating.] #speaker=Arthur #left_portraits=heidi_half #right_portraits=arthur

Bobby, our situation is what it is. The entire capital isn't feasting while we suffer, I'm sure they're doing what they can. #speaker=Arthur #left_portraits=heidi_half #right_portraits=arthur

[Nodding slightly.] #speaker=Heidi #left_portraits=heidi_half #right_portraits=arthur

Thank you for understanding sir. Only a few of us are dining on caviar, the rest are working until our shift is up. #speaker=Heidi #left_portraits=heidi_half #right_portraits=arthur

Her crew stiffen at the comment, amazed at her inability to read the room. #left_portraits=heidi_half #right_portraits=arthur

Bobby, infuriated, steps close to Heidi, their faces inches apart. She stands her ground, expressionless. #left_portraits=heidi_half #right_portraits=arthur

~ PlayAudio("snowy_footstep")

Cooper (28yrs old) takes a few steps to stand closer to his captain, wary of termpers flaring. He knows Heidi's bluntness can spark misunderstandings, few truly understand her. #left_portraits=cooper_half,heidi_half #right_portraits=mara_half

[Pushing Bobby gently.] #speaker=Frank #left_portraits=cooper_half,heidi_half #right_portraits=mara_half

Come on, Bobby, let's board. I'll treat you to king crab and salmon when we get there. #speaker=Frank #left_portraits=cooper_half,heidi_half #right_portraits=mara_half

He nudges Bobby harder to keep him moving. #left_portraits=cooper_half,heidi_half #right_portraits=mara_half

~ PlayAudio("metal_door")

As villagers board, the heavy door slides shut with a tight slam, shielding the warmth inside from the frigid air.

~ ChangeScene("Blank")
~ FadeBackground("black", 3)
~ Delay(3)
~ FadeBackground("passenger_depot", 3)
~ Delay(3)
~ ChangeScene("Dialog")

[Reporting to Heidi.] #speaker=Olivia #right_portraits=olivia_half

Captain, with nearly 20 villagers, we can keep them in one passenger car for warmth. Everyone's disheveled but stable, no one's sick yet. With about 70 feet worth of space, we've got plenty of room. If you want, we can also split them up for even more room to further avoid the chance of disease spreading. #speaker=Olivia #right_portraits=olivia_half

Let's keep them together. Body heat will help, and we'll save on coal. With the long journey and unpredictable weather, the steam line could freeze if we push too hard. Plus, there's something you're overlooking. #speaker=Heidi #left_portraits=heidi_half #right_portraits=olivia_half

[Bewildered.] #speaker=Olivia #left_portraits=heidi_half #right_portraits=olivia_half

Overlooking? Hmm… #speaker=Olivia #left_portraits=heidi_half #right_portraits=olivia_half

Here's a hint: we're all human in the end. #speaker=Heidi #left_portraits=heidi_half #right_portraits=olivia_half

[Connecting the dots.] #speaker=Heidi #left_portraits=heidi_half #right_portraits=olivia_half

Oh! Even if we split them up, we could spread illness ourselves moving between cars. I was so focused on the villagers health, for a slight moment I forgot we're not immune ourselves, my apology. #speaker=Olivia #left_portraits=heidi_half #right_portraits=olivia_half

Your instincts were sharp, but keeping everyone together would be better for our situation. We can use the second car for emergencies later if sickness becomes rampant. As for now, it will reduce our movement and need to communicate between the two cars. #speaker=Heidi #left_portraits=heidi_half #right_portraits=olivia_half

You're right, Captain. We need to conserve our energy for what's ahead. #speaker=Olivia #left_portraits=heidi_half #right_portraits=olivia_half

[Dryly. Daydreaming] #speaker=Heidi #left_portraits=heidi_half #right_portraits=olivia_half

Conserve energy? I was thinking more along the lines of being just too lazy to walk between cars. Maybe we can ask HQ to install a moving walkway next time. Or jetpacks, in case we're stranded, then we can fly home. #speaker=Heidi #left_portraits=heidi_half #right_portraits=olivia_half

[Dumbfounded.] #speaker=Olivia #left_portraits=heidi_half #right_portraits=olivia_half

Captain… #speaker=Olivia #left_portraits=heidi_half #right_portraits=olivia_half

[Approaching Heidi.] #speaker=Farin #left_portraits=heidi_half #right_portraits=olivia_half,farin_half

Captain, I checked on everyone. Most feel stable. Should we read the infection safety rubric right now? #speaker=Farin #left_portraits=heidi_half #right_portraits=olivia_half,farin_half

Let's wait until tomorrow. They're exhausted, more information now would overwhelm them. Time it for lunch, so they can eat and listen. It'll be like watching a dull channel you're too lazy to change. #speaker=Heidi #left_portraits=heidi_half #right_portraits=olivia_half,farin_half

Ehhh… Captain, that's not very encouraging, Captain… #speaker=Olivia #left_portraits=heidi_half #right_portraits=olivia_half,farin_half

It's fine, Olivia. They'll have three lovely ladies presenting. Studies show that beauty distracts from a poorly told story. We should use this evolutionary advantage given to us. #speaker=Heidi #left_portraits=heidi_half #right_portraits=olivia_half,farin_half

Farin tried her hardest to suppress a smile. #left_portraits=heidi_half #right_portraits=olivia_half,farin_half

Olivia looks even more dumbfounded. #left_portraits=heidi_half #right_portraits=olivia_half,farin_half

~ PlayAudio("sara_tim_footstep")

Sara and Tim dash to Heidi, excitement lighting their faces. #left_portraits=heidi_half #right_portraits=sara_half,tim_half

Ma'am, will you play the trumpet again? Can we try it too? #speaker=Tim #left_portraits=heidi_half #right_portraits=sara_half,tim_half

I'll play soon, when we leave the village. You can listen, but you'll have to wait until a bit later once everyone is settled in, to try it yourselves, and only if you're on your best behavior. Can you do that? #speaker=Heidi #left_portraits=heidi_half #right_portraits=sara_half,tim_half

[Nodding eagerly.] #speaker=Sara & Tim #left_portraits=heidi_half #right_portraits=sara_half,tim_half

Yes! #speaker=Sara & Tim #left_portraits=heidi_half #right_portraits=sara_half,tim_half

Good, help Olivia and Farin here get everyone settled. The faster we're ready, the sooner I'll play. #speaker=Heidi & Tim #left_portraits=heidi_half #right_portraits=sara_half,tim_half

~ PlayAudio("sara_tim_footstep")

Sara and Tim exchange glances, then dash to Olivia and Farin, insisting on helping. #left_portraits=heidi_half #right_portraits=sara_half,tim_half
