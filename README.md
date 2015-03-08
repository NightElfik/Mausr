Mausr
=================================
Marek's Unicode Symbol Recognizer
---------------------------------
Mausr is a neural network library written from scratch and used for recognition of hand written unicode symbols.
The library is relatively general and written with emphasis on datastructures, and extensibility (and maybe a little of performacne, too :).

The main reason behind this project was my personal interest in neural networks.
I decided to create unicode symbol recognizer because I often find myslef googling for an unicode symbol and it takes too much time.


**Author**: Marek Fiser &lt; mausr@marekfiser.cz &gt;

**Running instance**: http://www.mausr.com/

**License**: The MIT license, see LICENSE.txt for details.


Main features of neural net library
-------------
* Basic neural network layout with input layer, output layer, and any number of hidden layers.
* Extensible neuron activation function, stadnard sigmoid function implemented.
* Extensible net cost function, standard logistic regression cost function implemented.
* Extensible gradient based optimization algorithms with visual and algorithmical tests, implemented three:
  * Basic gradient descent,
  * Gradient descent with momentum, and
  * RProp- algorithm.
* Standard back-propagation learning algorithm.
  * Efficient, vectorized, and paralellized implementation.
  * Regularization implemented to avoid overfitting.
* Contains around 25 unit tests that ensure correctness of core components of training and evaluation algorithms.
  * Also contains simple visual tests of optimization algorithms to ensure expected behavior.

Main features of web interface
-------------
* Search for hand drawn symbols using canvas.
* Interface for neural network settings and training.
  * Includes real-time visual feedback of traind and test validation erors using signal-r and google chart API.
* Interface for training of new symbol drawings.
* Interface for approving anonymously submitted trainng data.
* Database for storing symbols, drawings, and users.
