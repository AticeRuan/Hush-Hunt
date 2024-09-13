using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Maui.Audio;

namespace HushHunt.Maui.Models
{
    public class SoundManager
    {
        private static SoundManager _instance;
        private IAudioManager _audioManager;
        private IAudioPlayer _soundPlayer;
        private IAudioPlayer _musicPlayer;
        private bool _isSoundEnabled = true;
        private bool _isMusicEnabled = true;

        public bool IsMusicEnabled => _isMusicEnabled;
        public bool IsSoundEnabled => _isSoundEnabled;

        private SoundManager() { }

        public static SoundManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SoundManager();
                }
                return _instance;
            }
        }

        public void Initialize(IAudioManager audioManager)
        {
            _audioManager = audioManager;
        }

        public void PlaySound(string soundFileName,bool loop=false, double volume = 0.5)
        {
            if (_isSoundEnabled)
            {
                if (_audioManager == null)
                {
                    throw new InvalidOperationException("AudioManager is not initialized.");
                }



                var sound = FileSystem.OpenAppPackageFileAsync(soundFileName).Result;
                if (sound == null)
                {
                    throw new FileNotFoundException("Sound file not found.", soundFileName);
                }
                _soundPlayer = _audioManager.CreatePlayer(sound);
                                _soundPlayer.Loop = loop;
                _soundPlayer.Volume = volume;
                _soundPlayer.Play();
                
               
            }
        }


        public void PlayMusic(string musicFileName, bool loop = false,double volume=0.1)
        {
            if (_isSoundEnabled)
            {
                if (_audioManager == null)
                {
                    throw new InvalidOperationException("AudioManager is not initialized.");
                }



                var music = FileSystem.OpenAppPackageFileAsync(musicFileName).Result;
                if (music == null)
                {
                    throw new FileNotFoundException("Sound file not found.", musicFileName);
                }
                _musicPlayer = _audioManager.CreatePlayer(music);
                _musicPlayer.Loop = loop;
                _musicPlayer.Volume = volume;
                _musicPlayer.Play();


            }
        }

        public void MuteSound()
        {
            _soundPlayer.Volume = 0;
       
        }

        public void ResetSound()
        {
            _soundPlayer.Volume = 0.2;
        }

        public void MuteMusic()
        {
            _musicPlayer.Volume = 0;

        }

        public void ResetMusic()
        {
            _musicPlayer.Volume = 0.2;
        }




        public void ToggleSound(bool isEnabled)
        {
            if (isEnabled != _isSoundEnabled)
            {
                _isSoundEnabled = isEnabled;

                if (!_isSoundEnabled)
                {
                    MuteSound(); 
                }
                else
                {
                    ResetSound();
                }
            }
        }

        public void ToggleMusic(bool isEnabled)
        {
            if (isEnabled != _isMusicEnabled)
            {
                _isMusicEnabled = isEnabled;

                if (!_isMusicEnabled)
                {
                    MuteMusic();
                }
                else
                {
                    ResetMusic();
                }
            }
        }


    }
}
