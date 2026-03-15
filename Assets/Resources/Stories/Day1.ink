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

[Looking down, wringing her hands. Her voice trembles, eyes scanning the white horizon.] #speaker=Sara #portrait=sara_half

When are they coming? #speaker=Sara #portrait=sara_half

[Shivering, clutching a thin blanket, tugging at two nearby adults. His lips are chapped from the cold.] #speaker=Tim #portrait=tim_half

Are they really coming? #speaker=Tim #portrait=tim_half

[Kneeling, forcing a smile, though his eyes betray doubt.] #speaker=Frank #portrait=frank_half

Don't worry, Tim, I'm sure they'll come. [His hand rests on Tim's head.] #speaker=Frank #portrait=frank_half

[Arms crossed, staring at the ground, voice flat and resigned.] #speaker=Bobby #portrait=bobby_half

Hmph. The capital starts the mess, then takes its sweet time fixing it. Bet they've forgotten us. #speaker=Bobby #portrait=bobby_half

[Hunched slightly, shivering to generate any warmth possible.] #speaker=Mara #portrait=mara_half

We're out of supplies and options. All we can do is stay optimistic and wait. #speaker=Mara #portrait=mara_half

[Snapping, fists clenched.] #speaker=Bobby #portrait=bobby_half

It's this damn war's fault! We didn't ask for it, yet we're the ones suffering. One idiot glares at another, and next thing you know, we're all buried in snow. #speaker=Bobby #portrait=bobby_half

His words echo briefly, then fade into the drifts.

[Huddling closer to Sara, his small voice lost in the wind.] #speaker=Tim #portrait=tim_half

I'm cold… and hungry… #speaker=Tim #portrait=tim_half

[Standing tall, voice booming despite his frail frame.] #speaker=Arthur #portrait=arthur

Everyone! Let's gather and pray for a new life, for the seasons to turn again, for a sign from above to carry us forward! #speaker=Arthur #portrait=arthur

He raises his hands, inviting the villagers closer. Villagers shuffle closer, forming a rough circle. Hands clasp, lips move silently as snow dances around them.

~ SetAudioVolume("dragonwind", 1)

The wind howls louder, drowning murmurs. Then… silence... A deafening void swallows the village.

~ SetAudioVolume("dragonwind", 0.2)

[Looking over the crowd with warm, steady eyes.] #speaker=Arthur #portrait=arthur

My friends, my family, set aside your burdens for a moment. I know we're tired and scared. The days are long, and help feels far. But we are not alone, not now, not ever. The Lord is with us, calling us to stand together. Will you pray with me? #speaker=Arthur #portrait=arthur

Villagers nod, some clasping hands, others bowing heads. Arthur raises his chin, signaling the start of the prayer.

Heavenly Father, in this hour of trial, we turn to You, our rock and refuge. Grant us strength to endure, courage to face our fears, and faith to trust Your plan. Surround us with Your love, unite us in hope, and guide us through this storm. May Your peace fill our hearts as we await Your deliverance. Amen. #speaker=Arthur #portrait=arthur

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
~ Delay(45)
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

[Standing firm, coat crisp, expression calm but eyes warm. A trumpet neatly strap to her coat.] #speaker=Heidi #portrait=heidi_half

I'm sorry for the delay. Let us travel together, toward home. #speaker=Heidi #portrait=heidi_half

Her voice is steady, kind, but measured, cutting through the winter silence.

The passenger car door flings open, revealing three crew members. Two more trot down from the locomotive to meet the new guests.

Tim and Sara dash toward Heidi, their faces bright with renewed energy.

[Walking up to Heidi, relieved.] #speaker=Arthur #portrait=heidi_half,arthur_half

I'm so glad you came. We feared help wouldn't arrive. #speaker=Arthur #portrait=heidi_half,arthur_half

I apologize again for the delay. It takes longer than we'd like to complete mission briefings and secure supplies for such a journey. We'd prefer to depart at a moment's notice, but that's rarely possible. #speaker=Heidi #portrait=heidi_half,arthur_half

~ PlayAudio("snowy_footstep")

[Approaching grumpily.] #speaker=Bobby #portrait=heidi_half,bobby_half

You took your damn time, as far as I'm concerned. #speaker=Bobby #portrait=heidi_half,bobby_half

[Apologetic] #speaker=Heidi #portrait=heidi_half,bobby_half

My sincerest apology once more. Protocol caused some delay. We— #speaker=Heidi #portrait=heidi_half,bobby_half

[Annoyed] #speaker=Bobby #portrait=heidi_half,bobby_half

Why didn't you just wait until Sunday to come. At least on Sunday you would have had an excuse to take your damn sweet time #speaker=Bobby #portrait=heidi_half,bobby_half

[With dry humor, unfazed.] #speaker=Heidi #portrait=heidi_half,bobby_half

\*\*Sigh\*\*, Your opinion has been noted sir. Next time, we'll make sure we arrive Sunday. If you want, you can wait while everyone else board, and we'll come back Sunday. Perhaps even add a cherry on top of your Sunday when we arrive. #speaker=Heidi #portrait=heidi_half,bobby_half
